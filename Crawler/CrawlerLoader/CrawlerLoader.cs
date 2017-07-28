using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using TreeSharp;
using Action = System.Action;


namespace CrawlerLoader
{
    public class CrawlerLoader : BotBase
    {
        // Change this settings to reflect your project!
        private const int ProjectId = 5;
        private const string ProjectName = "Crawler";
        private const string ProjectMainType = "Crawler.Crawler";
        private const string ProjectAssemblyName = "Crawler.dll";
        private static readonly Color LogColor = Colors.Red;
        public override PulseFlags PulseFlags => PulseFlags.All;
        public override bool IsAutonomous => false;
        public override bool WantButton => true;
        public override bool RequiresProfile => false;

        #region Meta Data

        // Don't touch anything else below from here!
        private static readonly object Locker = new object();
        private static readonly string ProjectAssembly = Path.Combine(Environment.CurrentDirectory, $@"BotBases\{ProjectName}\{ProjectAssemblyName}");
        private static readonly string GreyMagicAssembly = Path.Combine(Environment.CurrentDirectory, @"GreyMagic.dll");
        private static readonly string VersionPath = Path.Combine(Environment.CurrentDirectory, $@"BotBases\{ProjectName}\version.txt");
        private static readonly string BaseDir = Path.Combine(Environment.CurrentDirectory, $@"BotBases\{ProjectName}");
        private static readonly string ProjectTypeFolder = Path.Combine(Environment.CurrentDirectory, @"BotBases");
        private static bool _updated;
        private static readonly Composite FailsafeRoot = new TreeSharp.Action(c =>
        {
            Log($"{ProjectName} is not loaded correctly.");
            TreeRoot.Stop();
        });

        #endregion

        #region Constructor

        public CrawlerLoader()
        {
            lock (Locker)
            {
                if (_updated) { return; }
                _updated = true;
            }

            Task.Factory.StartNew(Update);
        }

        #endregion

        #region Overrides

        public override string Name => ProjectName;

        public override Composite Root => _root ?? FailsafeRoot;

        public override void OnButtonPress() => _onButtonPress?.Invoke();

        public override void Start() => _start?.Invoke();

        public override void Stop() => _stop?.Invoke();

        #endregion

        #region Injections

        private static Composite _root;
        private static Action _onButtonPress, _start, _stop;

        #endregion

        #region Injection Methods

        private static void Load()
        {
            RedirectAssembly();

            var assembly = LoadAssembly(ProjectAssembly);
            if (assembly == null)
            {
                return;
            }

            Type baseType;
            try
            {
                baseType = assembly.GetType(ProjectMainType);
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return;
            }

            object botBase;
            try
            {
                botBase = Activator.CreateInstance(baseType);
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return;
            }

            if (botBase == null)
            {
                Log("[Error] Could not load main type.");
                return;
            }

            var type = botBase.GetType();
            _root = (Composite)type.GetProperty("Root").GetValue(botBase);
            _start = (Action)type.GetProperty("StartAction").GetValue(botBase);
            _stop = (Action)type.GetProperty("StopAction").GetValue(botBase);
            _onButtonPress = (Action)type.GetProperty("ButtonAction").GetValue(botBase);

            Log($"{ProjectName} loaded.");
        }
        private static Assembly LoadAssembly(string path)
        {
            if (!File.Exists(path)) { return null; }

            Assembly assembly = null;
            try { assembly = Assembly.LoadFrom(path); }
            catch (Exception e) { Logging.WriteException(e); }

            return assembly;
        }

        #endregion

        #region Automatic Update Methods

        private static void Update()
        {
            var stopwatch = Stopwatch.StartNew();
            var local = GetLocalVersion();
            var message = new VersionMessage { LocalVersion = local, ProductId = ProjectId };
            var responseMessage = GetLatestVersion(message).Result;
            var latest = responseMessage?.LatestVersion;

            if (local == latest || latest == null)
            {
                Load();
                return;
            }

            Log($"Updating to {latest}.");
            var bytes = responseMessage.Data;

            if (bytes == null || bytes.Length == 0)
            {
                Log("[Error] Bad product data returned.");
                return;
            }

            if (!Clean(BaseDir))
            {
                Log("[Error] Could not clean directory for update.");
                return;
            }

            if (!Extract(bytes, ProjectTypeFolder))
            {
                Log("[Error] Could not extract new files.");
                return;
            }

            if (File.Exists(VersionPath)) { File.Delete(VersionPath); }
            try { File.WriteAllText(VersionPath, latest); }
            catch (Exception e) { Log(e.ToString()); }

            stopwatch.Stop();
            Log($"Update complete in {stopwatch.ElapsedMilliseconds} ms.");
            Load();
        }
        private static string GetLocalVersion()
        {
            if (!File.Exists(VersionPath)) { return null; }
            try
            {
                var version = File.ReadAllText(VersionPath);
                return version;
            }
            catch { return null; }
        }
        private static bool Clean(string directory)
        {
            foreach (var file in new DirectoryInfo(directory).GetFiles())
            {
                try { file.Delete(); }
                catch { return false; }
            }

            foreach (var dir in new DirectoryInfo(directory).GetDirectories())
            {
                try { dir.Delete(true); }
                catch { return false; }
            }

            return true;
        }
        private static bool Extract(byte[] files, string directory)
        {
            using (var stream = new MemoryStream(files))
            {
                var zip = new FastZip();

                try { zip.ExtractZip(stream, directory, FastZip.Overwrite.Always, null, null, null, false, true); }
                catch (Exception e)
                {
                    Log(e.ToString());
                    return false;
                }
            }

            return true;
        }
        private static async Task<VersionMessage> GetLatestVersion(VersionMessage message)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://auth.magitek.io");

                var json = JsonConvert.SerializeObject(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                try
                {
                    response = await client.PostAsync("/products/version", content);
                }
                catch (Exception e)
                {
                    Log(e.Message);
                    return null;
                }

                var contents = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<VersionMessage>(contents);
                return responseObject;
            }
        }

        #endregion

        #region Helpers

        private static void Log(string message)
        {
            Logging.Write(LogColor, $"[Auto-Updater][{ProjectName}] {message}");
        }
        public static void RedirectAssembly()
        {
            ResolveEventHandler handler = (sender, args) =>
            {
                var name = Assembly.GetEntryAssembly().GetName().Name;
                var requestedAssembly = new AssemblyName(args.Name);
                return requestedAssembly.Name != name ? null : Assembly.GetEntryAssembly();
            };

            AppDomain.CurrentDomain.AssemblyResolve += handler;

            ResolveEventHandler greyMagicHandler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                return requestedAssembly.Name != "GreyMagic" ? null : Assembly.LoadFrom(GreyMagicAssembly);
            };

            AppDomain.CurrentDomain.AssemblyResolve += greyMagicHandler;
        }
        public class VersionMessage
        {
            public int ProductId { get; set; }
            public string LocalVersion { get; set; }
            public string LatestVersion { get; set; }
            public byte[] Data { get; set; } = new byte[0];
        }

        #endregion
    }
}