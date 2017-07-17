using System;
using System.Windows.Forms;
using System.Windows.Input;
using ff14bot.Managers;

namespace Crawler.Utilities
{
    public class HotKey
    {
        public string Name { get; set; }

        public Keys Key { get; set; }

        public ModifierKeys ModifierKeyEnum { get; set; }

        public Action<Hotkey> Callback { get; set; }
    }
}