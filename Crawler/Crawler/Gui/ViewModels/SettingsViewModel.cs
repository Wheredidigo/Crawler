using System.Windows.Input;
using Crawler.Gui.Commands;
using PropertyChanged;

namespace Crawler.Gui.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new DelegateCommand(() => { Crawler.SettingsForm.Close(); }));
    }
}