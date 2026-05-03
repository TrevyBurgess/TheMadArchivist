using CyberFeedForward.TheMadArchivist.Services;
using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace CyberFeedForward.TheMadArchivist.Views.Controls;

public sealed partial class ArchiveListControl : UserControl
{
    public ArchiveListControl()
    {
        InitializeComponent();
        ViewModel = new ArchiveListControlViewModel(new ArchivesSettingsService(new LocalAppSettingsStore()));
    }

    public ArchiveListControlViewModel ViewModel
    {
        get => (ArchiveListControlViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(ArchiveListControlViewModel),
            typeof(ArchiveListControl),
            new PropertyMetadata(null));

    private void AddArchiveButton_OnClick(object _, RoutedEventArgs e)
    {
        TryAddTypedFolderPathToArchives();
    }

    private void NewArchivePathTextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != Windows.System.VirtualKey.Enter)
        {
            return;
        }

        e.Handled = true;
        TryAddTypedFolderPathToArchives();
    }

    private void NewArchivePathTextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        TryAddTypedFolderPathToArchives();
    }

    private void TryAddTypedFolderPathToArchives()
    {
        if (ViewModel is null)
        {
            return;
        }

        var candidatePath = ViewModel.NewArchivePath?.Trim();
        if (string.IsNullOrWhiteSpace(candidatePath))
        {
            return;
        }

        var result = ViewModel.TryAddFolderPath(candidatePath);

        if (App.MainWindowInstance is not CyberFeedForward.TheMadArchivist.MainWindow mainWindow)
        {
            return;
        }

        switch (result)
        {
            case ArchiveListControlViewModel.ArchiveAddResult.Added:
                mainWindow.SetStatusText("Folder Added");
                break;
            case ArchiveListControlViewModel.ArchiveAddResult.Duplicate:
                mainWindow.SetStatusText($"Archive already exists: {candidatePath}");
                break;
            case ArchiveListControlViewModel.ArchiveAddResult.NotFound:
                mainWindow.SetStatusText($"Folder not found: {candidatePath}");
                break;
            case ArchiveListControlViewModel.ArchiveAddResult.Error:
                mainWindow.SetStatusText($"Error adding folder: {candidatePath}");
                break;
            case ArchiveListControlViewModel.ArchiveAddResult.Empty:
            default:
                break;
        }
    }

    private async void BrowseArchiveButton_OnClick(object _, RoutedEventArgs e)
    {
        var picker = new FolderPicker();
        picker.FileTypeFilter.Add("*");

        if (App.MainWindowInstance is null)
        {
            return;
        }

        var hwnd = WindowNative.GetWindowHandle(App.MainWindowInstance);
        InitializeWithWindow.Initialize(picker, hwnd);

        StorageFolder? folder;
        try
        {
            folder = await picker.PickSingleFolderAsync();
        }
        catch
        {
            return;
        }

        if (folder is null)
        {
            return;
        }

        ViewModel.NewArchivePath = folder.Path;
        TryAddTypedFolderPathToArchives();
    }

    private async void RemoveArchiveItemButton_OnClick(object sender, RoutedEventArgs _)
    {
        if (sender is not FrameworkElement fe)
        {
            return;
        }

        if (fe.DataContext is not string archivePath)
        {
            return;
        }

        var dialog = new ContentDialog
        {
            Title = "Remove folder?",
            Content = $"Remove this folder from the archives list?\n\n{archivePath}",
            PrimaryButtonText = "Remove",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = XamlRoot,
        };

        var result = await dialog.ShowAsync();
        if (result != ContentDialogResult.Primary)
        {
            return;
        }

        var removed = ViewModel?.RemoveArchive(archivePath) == true;
        if (!removed)
        {
            return;
        }

        if (App.MainWindowInstance is CyberFeedForward.TheMadArchivist.MainWindow mainWindow)
        {
            mainWindow.SetStatusText("Folder Deleted");
        }
    }
}
