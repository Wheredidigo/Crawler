using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ff14bot.Helpers;
using Newtonsoft.Json;

namespace Crawler.AuthCore
{
    public class Authentication
    {
        private const string BaseAddress = "https://magitek.io";
        private static readonly object Locker = new object(), DelayLocker = new object();
        private static volatile bool _started, _exit, _forceRefresh;
        private static readonly Stopwatch DelayWatch = new Stopwatch();

        static Authentication()
        {
            if (_started)
            {
                return;
            }

            _started = true;
            Task.Factory.StartNew(KeepSession);
        }

        private static ConcurrentDictionary<int, ClientSession> ProductSessions { get; } = new ConcurrentDictionary<int, ClientSession>();
        private static ConcurrentDictionary<int, string> Products { get; } = new ConcurrentDictionary<int, string>();
        private static ConcurrentDictionary<int, Action<string>> LogFunctions { get; } = new ConcurrentDictionary<int, Action<string>>();

        private static Action<string> Log { get; } = Logging.Write;

        private static bool ForceRefresh
        {
            get
            {
                if (!_forceRefresh)
                {
                    return false;
                }

                lock (DelayLocker)
                {
                    if (!DelayWatch.IsRunning)
                    {
                        return true;
                    }

                    if (DelayWatch.ElapsedMilliseconds < 1000)
                    {
                        return false;
                    }

                    DelayWatch.Reset();
                }

                return true;
            }
        }

        public static void Start()
        {
            lock (Locker)
            {
                if (_started)
                {
                    return;
                }

                _started = true;

                // This is started in the constructor, this method might be useless?
                Task.Factory.StartNew(KeepSession);
            }
        }

        public static void Stop()
        {
            _exit = true;
        }

        public static void RegisterProduct(int product, string name, Action<string> log = null)
        {
            if (string.IsNullOrWhiteSpace(name) || product == 0)
            {
                return;
            }

            // Add the product to the products dictionary 
            Products[product] = name;

            if (log != null)
            {
                LogFunctions[product] = log;
            }
        }

        public static void SetProduct(int product, string key)
        {
            // If the key is null or the product doesn't exist return
            if (string.IsNullOrWhiteSpace(key) || product == 0) { return; }

            // If the product is already part of the product sessions dictionary and the same session exists, just return
            if (ProductSessions.TryGetValue(product, out ClientSession session) && session.Key == key)
            {
                return;
            }

            // We got the product, but the session doesn't exist, create the session
            session = new ClientSession { Key = key, Product = product };

            // Set the current product to the new session
            ProductSessions[product] = session;

            // Force the refresh
            _forceRefresh = true;

            // Reset the timer
            lock (DelayLocker)
            {
                DelayWatch.Restart();
            }
        }

        public static async Task RemoveProduct(int product)
        {
            // If we fail to remove (probably cause it doesnt exist) the session or if the session isn't active, then just return
            if (!ProductSessions.TryRemove(product, out ClientSession session) || !session.Active)
            {
                return;
            }

            // create a new list of sessions and then call logout to remove the session
            var sessions = new List<ClientSession> { session };
            await LogoutSessions(sessions, "/api/keys/logout");
        }

        public static bool IsAuthenticated(int product)
        {
            // Get the current session
            ProductSessions.TryGetValue(product, out ClientSession session);

            // Return true if the session exists and it is considered active
            return session != null && session.Active;
        }

        private static async void KeepSession()
        {
            await Task.Delay(500);

            // Keep looping this shit as long as we're not supposed to logout
            while (!_exit)
            {
                _forceRefresh = false;

                // Convert the current dictionary into a list
                var sessions = ProductSessions.Values.ToList();

                // Create a new list from the current sessions; if none exist, create a new list of client sessions
                var refreshed = sessions.Count != 0
                    ? await JsonRefreshSessions(sessions, "/api/keys/refresh")
                    : new List<ClientSession>();

                // api/keys/refresh returns an array of ClientSession objects .... example
                //[
                //    {
                //        "guid": null,
                //        "key": "d5bc39ca-a49c-4952-b99c-cb255a7ccb64",
                //        "product": 1,
                //        "active": false,
                //        "message": "Your key does not exist."
                //    }
                //]

                // go through all of the sessions in the list
                foreach (var r in refreshed)
                {
                    // find the matching session in the old sessions list
                    var prev = sessions.FirstOrDefault(s => s.Key == r.Key);

                    // set as true if the previous sessions was active
                    var wasActive = prev?.Active ?? false;

                    // If it was active but it isn't anymore, print logged out cause that session is gonna die
                    if (wasActive && !r.Active)
                    {
                        Write(r.Product, "Logged out.");
                    }

                    // If it wasn't active but now it is, print logged in because the user has just created a new session
                    if (!wasActive && r.Active)
                    {
                        Write(r.Product, "Logged in.");
                    }

                    // If the current session isn't active and the message doesn't match the message from the old session, print it
                    if (!r.Active && r.Message != prev?.Message && r.Message != null)
                    {
                        Write(r.Product, r.Message);
                    }

                    // Overrite the session in the Sessions dictionary with the current session
                    ProductSessions[r.Product] = r;
                }

                // Refresh every 1 minute (60 seconds)
                Wait(60000);
            }

            var active = ProductSessions.Values.Where(v => v.Active).ToList();
            await LogoutSessions(active, "/api/keys/logout");
        }

        private static void Wait(long timeMs)
        {
            var stopwatch = Stopwatch.StartNew();

            while (!_exit && !ForceRefresh && stopwatch.ElapsedMilliseconds < timeMs)
            {
                Thread.Sleep(100);
            }
        }

        private static async Task<List<ClientSession>> JsonRefreshSessions(List<ClientSession> message, string route)
        {
            using (var client = new HttpClient())
            {
                // Set the base address for the service.
                client.BaseAddress = new Uri(BaseAddress);

                //Set any header information.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string json = JsonConvert.SerializeObject(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Actually make the Post call to the API.
                HttpResponseMessage response;
                try { response = await client.PostAsync(route, content); }
                catch (Exception e)
                {
                    Log?.Invoke($"[AuthCore] {e.Message}");
                    foreach (var s in message) { s.Active = false; }
                    return message;
                }

                // Make sure that the call was successful.
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log?.Invoke($"[AuthCore] {response.StatusCode}: {response.ReasonPhrase}");
                    foreach (var s in message) { s.Active = false; }
                    return message;
                }

                // Read the response content.
                string retval;
                try { retval = await response.Content.ReadAsStringAsync(); }
                catch (Exception e)
                {
                    Log?.Invoke($"[AuthCore] {e.Message}");
                    foreach (var s in message) { s.Active = false; }
                    return message;
                }

                List<ClientSession> returnMessage;
                try { returnMessage = JsonConvert.DeserializeObject<List<ClientSession>>(retval); }
                catch (Exception e)
                {
                    Log?.Invoke($"[AuthCore] {e.Message}");
                    foreach (var s in message) { s.Active = false; }
                    return message;
                }

                return returnMessage;
            }
        }

        private static async Task LogoutSessions(IReadOnlyCollection<ClientSession> message, string route)
        {
            using (var client = new HttpClient())
            {
                // Set the base address for the service.
                client.BaseAddress = new Uri(BaseAddress);

                //Set any header information.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Actually make the Post call to the API.
                HttpResponseMessage response;
                try { response = await client.PostAsync(route, content); }
                catch (Exception e)
                {
                    Log?.Invoke($"[AuthCore] {e.Message}");
                    return;
                }

                // Make sure that the call was successful.
                if (response.StatusCode != HttpStatusCode.OK) { Log?.Invoke($"[AuthCore] {response.StatusCode}: {response.ReasonPhrase}"); }
            }
        }

        private static void Write(int product, string message)
        {
            if (!Products.TryGetValue(product, out string name))
            {
                name = product.ToString();
            }

            if (LogFunctions.TryGetValue(product, out Action<string> log))
            {
                log.Invoke(message);
            }
            else
            {
                Log.Invoke($"[{name}] {message}");
            }
        }
    }
}
