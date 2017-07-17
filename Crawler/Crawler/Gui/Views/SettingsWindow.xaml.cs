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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}