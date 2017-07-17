using System.Windows.Input;
using Crawler.Gui.Commands;
using Crawler.Settings;
using PropertyChanged;

namespace Crawler.Gui.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        private static SettingsViewModel _instance;
        public static SettingsViewModel Instance => _instance ?? (_instance = new SettingsViewModel());

        public static CrawlerSettings CrawlerSettings => CrawlerSettings.Instance;

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new DelegateCommand(() => { Crawler.SettingsForm.Close(); }));
    }
}