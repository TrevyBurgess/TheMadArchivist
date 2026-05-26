using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using CyberFeedForward.TheMadArchivist.AppTools.FileSystem;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
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
}
