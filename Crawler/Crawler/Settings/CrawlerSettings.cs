using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using System.Windows.Input;
using Crawler.AuthCore;
using Crawler.Utilities;
using ff14bot.Helpers;
using Newtonsoft.Json;
using PropertyChanged;

namespace Crawler.Settings
{
    [AddINotifyPropertyChangedInterface]
    public class CrawlerSettings : JsonSettings
    {
        public CrawlerSettings() : base(SettingsPath + "/Crawler/Settings.json")
        {
        }

        public static CrawlerSettings Instance => _instance ?? (_instance = new CrawlerSettings());
        [JsonIgnore]
        private static CrawlerSettings _instance;

        [Setting]
        public string ProductKey
        {
            get { return _productKey; }
            set
            {
                Authentication.SetProduct(5, value);
                _productKey = value;
                Save();
            }
        }
        private string _productKey = "";

        [Setting]
        [DefaultValue(false)]
        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                _isPaused = value;
                Save();
            }
        }
        private bool _isPaused;

        [Setting]
        [DefaultValue(true)]
        public bool UseOverlay
        {
            get { return _useOverlay; }
            set
            {
                _useOverlay = value;
                Save();
            }
        }
        private bool _useOverlay = true;

        [Setting]
        [DefaultValue(60)]
        public double OverlayPosX { get; set; }

        [Setting]
        [DefaultValue(60)]
        public double OverlayPosY { get; set; }

        private double _overlayWidth;

        [Setting]
        [DefaultValue(200)]
        public double OverlayWidth
        {
            get { return _overlayWidth; }
            set
            {
                _overlayWidth = value;
                Save();
            }
        }

        [Setting]
        [DefaultValue(true)]
        public bool UseAutoTargeting
        {
            get { return _useAutoTargeting; }
            set
            {
                _useAutoTargeting = value;
                Save();
            }
        }

        private double _overlayBackgroundOpacity;

        [Setting]
        [DefaultValue(0.8)]
        public double OverlayBackgroundOpacity
        {
            get { return _overlayBackgroundOpacity; }
            set
            {
                _overlayBackgroundOpacity = value;
                Save();
            }
        }

        private bool _useAutoTargeting;

        [Setting]
        [DefaultValue(true)]
        public bool SkipCutscenes
        {
            get { return _skipCutscenes; }
            set
            {
                _skipCutscenes = value;
                Save();
            }
        }
        private bool _skipCutscenes;

        [Setting]
        [DefaultValue(true)]
        public bool SkipDialog
        {
            get { return _skipDialog; }
            set
            {
                _skipDialog = value;
                Save();
            }
        }
        private bool _skipDialog;

        [Setting]
        [DefaultValue(true)]
        public bool AcceptQuests
        {
            get { return _acceptQuests; }
            set
            {
                _acceptQuests = value;
                Save();
            }
        }
        private bool _acceptQuests;

        [Setting]
        [DefaultValue(Keys.D0)]
        public Keys TogglePauseKey
        {
            get { return _togglePauseKey; }
            set
            {
                _togglePauseKey = value;
                TogglePauseCrHotKey.Key = value;
                EventHooks.UpdateHotKey(TogglePauseCrHotKey);
                Save();
            }
        }
        private Keys _togglePauseKey;

        [Setting]
        [DefaultValue(ModifierKeys.Alt)]
        public ModifierKeys TogglePauseModifierKey
        {
            get { return _togglePauseModeifierKey; }
            set
            {
                _togglePauseModeifierKey = value;
                TogglePauseCrHotKey.ModifierKeyEnum = value;
                EventHooks.UpdateHotKey(TogglePauseCrHotKey);
                Save();
            }
        }
        private ModifierKeys _togglePauseModeifierKey;
        
        [JsonIgnore]
        private HotKey TogglePauseCrHotKey
        {
            get
            {
                return _togglePauseCrHotKey ?? (_togglePauseCrHotKey = new HotKey
                {
                    Name = "Crawler_Pause",
                    Key = Keys.D0,
                    ModifierKeyEnum = ModifierKeys.Alt,
                    Callback = hotkey =>
                    {
                        Instance.IsPaused = !IsPaused;
                        Logger.Log(Instance.IsPaused
                            ? "Pause HotKey Pressed. Crawler is Paused"
                            : "Pause HotKey Pressed. Crawler is Un-Paused");
                    }
                });
            }
        }
        private HotKey _togglePauseCrHotKey;

        public IEnumerable<HotKey> GetHotKeys()
        {
            return new[] { TogglePauseCrHotKey };
        }
    }
}