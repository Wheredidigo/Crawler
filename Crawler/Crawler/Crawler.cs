using Crawler.AuthCore;
using Crawler.Gui.Overlays;
using Crawler.Gui.Views;
using Crawler.Logic;
using Crawler.Settings;
using Crawler.Utilities;
using ff14bot;
using ff14bot.Navigation;
using TreeSharp;

namespace Crawler
{
    public class Crawler
    {
        #region Constructor

        public Crawler()
        {
            Authentication.RegisterProduct(5, "Crawler", Logger.Log);
            Authentication.SetProduct(5, CrawlerSettings.Instance.ProductKey);
            Root = new Decorator(r => Authentication.IsAuthenticated(5) && !CrawlerSettings.Instance.IsPaused && Core.Me.IsAlive,
                       new PrioritySelector(
                           Tank.Execute(),
                           Healer.Execute(),
                           Damage.Execute(),
                           Helpers.Execute(),
                           Hooks.Execute()));
            StartAction = Start;
            StopAction = Stop;
            ButtonAction = OnButtonPress;
        }

        #endregion

        #region Actions

        public System.Action StartAction { get; }
        public System.Action StopAction { get; }
        public System.Action ButtonAction { get; }

        #endregion

        #region Overrides

        public Composite Root { get; }
        public void Start()
        {
            Navigator.PlayerMover = new NullMover();
            Navigator.NavigationProvider = new NullProvider();
            EventHooks.RegisterHotKeys(CrawlerSettings.Instance.GetHotKeys());
            if(CrawlerSettings.Instance.UseOverlay) CrawlerOverlay.Start();
            Logger.Log("Starting Crawler");
        }
        public void Stop()
        {
            EventHooks.UnRegisterHotKeys(CrawlerSettings.Instance.GetHotKeys());
            CrawlerOverlay.Stop();
            Logger.Log("Stopping Crawler");
        }
        public void OnButtonPress()
        {
            Logger.Log("Opening Settings Window");
            SettingsForm.ShowDialog();
        }

        #endregion

        #region Gui

        private static SettingsWindow _settingsForm;
        internal static SettingsWindow SettingsForm
        {
            get
            {
                if (_settingsForm != null) return _settingsForm;
                _settingsForm = new SettingsWindow();
                _settingsForm.Closed += (sender, args) =>
                {
                    _settingsForm = null;
                    Logger.Log("Closing Settings Window");
                };
                return _settingsForm;
            }
        }
        
        #endregion
        
    }
}