using CyberFeedForward.TheMadArchivist.Services;
using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace CyberFeedForward.TheMadArchivist.Views.Controls;

public sealed partial class BreadcrumbBar : UserControl
{
    private readonly BreadcrumbBarViewModel _viewModel;

    public BreadcrumbBar()
    {
        InitializeComponent();
        _viewModel = new BreadcrumbBarViewModel(new FileSystemService());
    }

    public string? FolderPath
    {
        get
        {
            return (string?)GetValue(FolderPathProperty);
        }

        set
        {
            SetValue(FolderPathProperty, value);
        }
    }

    public static readonly DependencyProperty FolderPathProperty =
        DependencyProperty.Register(
            nameof(FolderPath),
            typeof(string),
            typeof(BreadcrumbBar),
            new PropertyMetadata(null, OnFolderPathChanged));

    public System.Collections.ObjectModel.ObservableCollection<BreadcrumbSegmentViewModel> Segments => _viewModel.Segments;

    private static void OnFolderPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BreadcrumbBar)d;
        control._viewModel.FolderPath = (string?)e.NewValue;
    }

    private void SegmentBreadcrumb_OnFolderPathSelected(object sender, string? selectedFolderPath)
    {
        if (string.IsNullOrWhiteSpace(selectedFolderPath))
        {
            return;
        }

        if (string.Equals(selectedFolderPath, FolderPath, StringComparison.Ordinal))
        {
            return;
        }

        FolderPath = selectedFolderPath;
    }
}
