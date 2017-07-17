using System.Windows.Input;
using Crawler.AuthCore;
using Crawler.Gui.Commands;
using Crawler.Settings;
using PropertyChanged;

namespace Crawler.Gui.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AuthenticationViewModel
    {
        private ICommand _testKeyCommand;
        public ICommand TestKeyCommand => _testKeyCommand ?? (_testKeyCommand = new DelegateCommand(TestKey, () =>
                                !string.IsNullOrWhiteSpace(CrawlerSettings.Instance.Email) &&
                                !string.IsNullOrWhiteSpace(CrawlerSettings.Instance.ProductKey)));

        public string Message { get; set; }
        public bool IsMessageVisible => !string.IsNullOrWhiteSpace(Message);

        private void TestKey()
        {
            Message = Authentication.IsAuthenticated(5) ? "Authenticated" : "Please try your key again.";
        }
    }
}