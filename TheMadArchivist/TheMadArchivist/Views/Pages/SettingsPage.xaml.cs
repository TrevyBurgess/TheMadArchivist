using Microsoft.UI.Xaml.Controls;

namespace CyberFeedForward.TheMadArchivist.Views.Pages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void AddArchiveButton_OnClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (DataContext is CyberFeedForward.TheMadArchivist.ViewModels.Pages.SettingsPageViewModel vm)
        {
            vm.AddArchive();
        }
    }

    private void RemoveArchiveButton_OnClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (DataContext is CyberFeedForward.TheMadArchivist.ViewModels.Pages.SettingsPageViewModel vm)
        {
            vm.RemoveSelectedArchive();
        }
    }
}
