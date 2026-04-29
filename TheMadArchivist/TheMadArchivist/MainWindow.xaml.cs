using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using CyberFeedForward.TheMadArchivist.Views.Pages;
using CyberFeedForward.TheMadArchivist.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CyberFeedForward.TheMadArchivist
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const string AppTitle = "The Mad Archivist";
        private readonly CommandBarSettingsService _commandBarSettings;

        public MainWindow()
        {
            InitializeComponent();

            _commandBarSettings = new CommandBarSettingsService(new LocalAppSettingsStore());
            SetCommandBarOnLeft(_commandBarSettings.IsCommandBarOnLeft());

            MainFrame.Navigated += MainFrame_OnNavigated;
            UpdateNavigationButtons();

            MainFrame.Navigate(typeof(HomePage));
        }

        private void MainFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateNavigationButtons();
            UpdateWindowTitle(e.SourcePageType);
            SetCommandBarOnLeft(_commandBarSettings.IsCommandBarOnLeft());
        }

        public void SetCommandBarOnLeft(bool onLeft)
        {
            MainCommandBar.SetValue(Grid.ColumnProperty, onLeft ? 0 : 2);
        }

        public void SetStatusText(string? text)
        {
            if (DispatcherQueue.HasThreadAccess)
            {
                StatusBarText.Text = string.IsNullOrWhiteSpace(text) ? "Ready" : text;
                return;
            }

            _ = DispatcherQueue.TryEnqueue(() =>
            {
                StatusBarText.Text = string.IsNullOrWhiteSpace(text) ? "Ready" : text;
            });
        }

        public void ClearStatusText()
        {
            SetStatusText(null);
        }

        private void UpdateWindowTitle(Type? pageType)
        {
            var pageTitle = GetPageTitle(pageType);
            Title = string.IsNullOrWhiteSpace(pageTitle) ? AppTitle : $"{AppTitle} - {pageTitle}";
        }

        private static string GetPageTitle(Type? pageType)
        {
            if (pageType is null)
            {
                return string.Empty;
            }

            var name = pageType.Name;
            if (name.EndsWith("Page", StringComparison.Ordinal))
            {
                name = name[..^4];
            }

            return name;
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

        private void HelpAboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(AboutPage));
        }
    }
}
