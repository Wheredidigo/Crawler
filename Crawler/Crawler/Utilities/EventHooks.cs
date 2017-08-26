using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ff14bot;
using ff14bot.Managers;

namespace Crawler.Utilities
{
    public static class EventHooks
    {
        public static void RegisterHotKeys(IEnumerable<HotKey> hotKeys, [CallerMemberName] string methodName = null)
        {
            if (BotManager.Current.Name == "Crawler")
            {
                Logger.Log($@"{methodName} was called. Adding HotKeys now!");

                //Get a list of all the currently registeredHotkeys to check against.
                var registeredHotkeys = new HashSet<string>(HotkeyManager.RegisteredHotkeys.Select(x => x.Name));

                foreach (var hotKey in hotKeys.Where(hotKey => !registeredHotkeys.Contains(hotKey.Name)))
                {
                    HotkeyManager.Register(hotKey.Name, hotKey.Key, hotKey.ModifierKeyEnum, hotKey.Callback);
                }
            }   
        }

        public static void UnRegisterHotKeys(IEnumerable<HotKey> hotKeys, [CallerMemberName] string methodName = null)
        {
            Logger.Log($@"{methodName} was called. Removing HotKeys now!");

            //Get a list of all the currently registeredHotkeys to check against.
            var registeredHotkeys = new HashSet<string>(HotkeyManager.RegisteredHotkeys.Select(x => x.Name));

            foreach (var hotKey in hotKeys.Where(hotKey => registeredHotkeys.Contains(hotKey.Name)))
            {
                while (HotkeyManager.RegisteredHotkeys.Select(x => x.Name).Contains(hotKey.Name))
                {
                    HotkeyManager.Unregister(hotKey.Name);
                }
            }
        }

        public static void UpdateHotKey(HotKey hotKey, [CallerMemberName] string methodName = null)
        {
            if (TreeRoot.IsRunning && BotManager.Current.Name == "Crawler")
            {
                Logger.Log($@"{methodName} was called. Updating {hotKey.Name} HotKey now!");

                while (HotkeyManager.RegisteredHotkeys.Select(x => x.Name).Contains(hotKey.Name))
                {
                    HotkeyManager.Unregister(hotKey.Name);
                }

                HotkeyManager.Register(hotKey.Name, hotKey.Key, hotKey.ModifierKeyEnum, hotKey.Callback);
            }
        }
    }
}