using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using System;
using System.IO;
using System.Runtime.InteropServices;
using CyberFeedForward.TheMadArchivist.Views.Pages;
using CyberFeedForward.TheMadArchivist.Services;
using CyberFeedForward.TheMadArchivist.ViewModels;
using CyberFeedForward.TheMadArchivist.Utilities;
using WinRT.Interop;

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
        private readonly MainWindowViewModel _viewModel;
        private readonly WindowPlacementSettingsService _windowPlacementSettings;
        private readonly AppWindow _appWindow;

        public MainWindow()
        {
            InitializeComponent();

            _commandBarSettings = new CommandBarSettingsService(new LocalAppSettingsStore());
            _viewModel = new MainWindowViewModel(_commandBarSettings, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            _windowPlacementSettings = new WindowPlacementSettingsService(new LocalAppSettingsStore());

            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            _appWindow = AppWindow.GetFromWindowId(windowId);

            SetWindowIcon();

            if (Content is FrameworkElement rootElement)
            {
                rootElement.DataContext = _viewModel;
            }
            SetCommandBarOnLeft(_commandBarSettings.IsCommandBarOnLeft());

            RestoreWindowPlacement(windowId);
            Closed += MainWindow_OnClosed;

            MainFrame.Navigated += MainFrame_OnNavigated;
            UpdateNavigationButtons();

            MainFrame.NavigateIfNotCurrent(typeof(SettingsPage));
        }

        private void SetWindowIcon()
        {
            var baseDir = AppContext.BaseDirectory;
            var iconPath = Path.Combine(baseDir, "Assets", "AppIcon.ico");

            if (!File.Exists(iconPath))
            {
                throw new InvalidOperationException($"Expected app icon file was not found: '{iconPath}'.");
            }

            _appWindow.SetIcon(iconPath);
        }

        private void RestoreWindowPlacement(WindowId windowId)
        {
            if (!_windowPlacementSettings.TryGetPlacement(out var placement))
            {
                return;
            }

            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
            var workArea = displayArea.WorkArea;

            var width = Math.Clamp(placement.Width, 200, workArea.Width);
            var height = Math.Clamp(placement.Height, 200, workArea.Height);

            var x = placement.X;
            var y = placement.Y;

            if (x < workArea.X)
            {
                x = workArea.X;
            }

            if (y < workArea.Y)
            {
                y = workArea.Y;
            }

            if (x + width > workArea.X + workArea.Width)
            {
                x = workArea.X + workArea.Width - width;
            }

            if (y + height > workArea.Y + workArea.Height)
            {
                y = workArea.Y + workArea.Height - height;
            }

            _appWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, width, height));
        }

        private void MainWindow_OnClosed(object sender, WindowEventArgs args)
        {
            var pos = _appWindow.Position;
            var size = _appWindow.Size;
            _windowPlacementSettings.SavePlacement(new WindowPlacement(pos.X, pos.Y, size.Width, size.Height));
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
                _viewModel.StatusText = string.IsNullOrWhiteSpace(text) ? "Ready" : text;
                return;
            }

            _ = DispatcherQueue.TryEnqueue(() =>
            {
                _viewModel.StatusText = string.IsNullOrWhiteSpace(text) ? "Ready" : text;
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
            MainFrame.NavigateIfNotCurrent(typeof(SettingsPage));
        }

        private void HelpAboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigateIfNotCurrent(typeof(AboutPage));
        }
    }
}
