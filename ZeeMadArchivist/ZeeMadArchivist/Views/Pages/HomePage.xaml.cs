using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

namespace CyberFeedForward.TheMadArchivist.Views.Pages;

public sealed partial class HomePage : Page
{
    private bool _isResizing;
    private double _startX;
    private double _startWidth;

    public HomePage()
    {
        InitializeComponent();
    }

    private void FolderContentsDivider_OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (sender is not UIElement divider)
        {
            return;
        }

        _isResizing = true;
        _startX = e.GetCurrentPoint(HomeRootGrid).Position.X;
        _startWidth = FolderContentsColumn.Width.Value;
        divider.CapturePointer(e.Pointer);
        e.Handled = true;
    }

    private void FolderContentsDivider_OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_isResizing)
        {
            return;
        }

        var x = e.GetCurrentPoint(HomeRootGrid).Position.X;
        var delta = x - _startX;
        var next = _startWidth + delta;

        var min = FolderContentsColumn.MinWidth;
        var max = Math.Max(min, HomeRootGrid.ActualWidth - 100);
        if (next < min)
        {
            next = min;
        }
        else if (next > max)
        {
            next = max;
        }

        FolderContentsColumn.Width = new GridLength(next);
        e.Handled = true;
    }

    private void FolderContentsDivider_OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        EndResize(sender as UIElement, e.Pointer);
        e.Handled = true;
    }

    private void FolderContentsDivider_OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
    {
        EndResize(sender as UIElement, e.Pointer);
        e.Handled = true;
    }

    private void EndResize(UIElement? divider, Pointer pointer)
    {
        if (!_isResizing)
        {
            return;
        }

        _isResizing = false;
        divider?.ReleasePointerCapture(pointer);
    }
}
