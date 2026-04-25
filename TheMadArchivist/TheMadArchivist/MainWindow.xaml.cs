using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using TheMadArchivist.Views.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TheMadArchivist
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigated += MainFrame_OnNavigated;
            UpdateNavigationButtons();

            MainFrame.Navigate(typeof(HomePage));
        }

        private void MainFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            BackCommandButton.IsEnabled = MainFrame.CanGoBack;
            ForwardCommandButton.IsEnabled = MainFrame.CanGoForward;
        }

        private void BackCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private void ForwardCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoForward)
            {
                MainFrame.GoForward();
            }
        }

        private void SettingsCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(SettingsPage));
        }
    }
}
