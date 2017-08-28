using System.Windows;
using System.Windows.Input;

namespace Crawler.Gui.Views
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InheritanceBehavior = InheritanceBehavior.SkipToThemeNext;
            InitializeComponent();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}