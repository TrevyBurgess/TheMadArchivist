using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using CyberFeedForward.TheMadArchivist.AppTools.FileSystem;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace CyberFeedForward.TheMadArchivist.Views.Controls;

public sealed partial class NamedIconControl : UserControl
{
    public NamedIconControl()
    {
        InitializeComponent();
        ViewModel = new NamedIconControlViewModel();
        ViewModel.LoadFromProgramData();
    }

    public NamedIconControlViewModel ViewModel
    {
        get => (NamedIconControlViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(NamedIconControlViewModel),
            typeof(NamedIconControl),
            new PropertyMetadata(null));

    private async void CustomIconsBrowseButton_OnClick(object sender, RoutedEventArgs e)
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

        if (ViewModel is null)
        {
            return;
        }

        ViewModel.CustomIconsFolderPath = folder.Path;
    }

    private void CustomIconsSaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.SaveCustomIconsFolderPath();
    }

    private void LoadDefaultIconsButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        FolderTools.LoadDefaultIcons(ViewModel.CustomIconsFolderPath);

        ViewModel.RefreshIcons();
    }

    private async void IconListRowSaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        if (sender is not Button button)
        {
            return;
        }

        if (button.DataContext is not IconListItemViewModel row)
        {
            return;
        }

        var newBaseName = row.Name;
        var originalFilePath = row.FilePath;

        var oldBaseName = string.IsNullOrWhiteSpace(originalFilePath)
            ? string.Empty
            : Path.GetFileNameWithoutExtension(originalFilePath);

        if (string.Equals(oldBaseName, newBaseName, StringComparison.Ordinal))
        {
            return;
        }

        var ok = FolderTools.TryRenameIconFile(
            ViewModel.CustomIconsFolderPath,
            originalFilePath,
            newBaseName,
            out var errorMessage);

        if (ok)
        {
            ViewModel.RefreshIcons();
            return;
        }

        var dialog = new ContentDialog
        {
            Title = "Rename Failed",
            Content = string.IsNullOrWhiteSpace(errorMessage) ? "Unable to rename icon." : errorMessage,
            CloseButtonText = "OK",
            XamlRoot = XamlRoot,
        };

        await dialog.ShowAsync();
    }

    private void IconListRowRevertButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.DataContext is not IconListItemViewModel row)
        {
            return;
        }

        row.Name = row.OriginalName;
    }
}
