using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CyberFeedForward.TheMadArchivist.Views.Controls;

public sealed partial class NamedIconControl : UserControl
{
    public NamedIconControl()
    {
        InitializeComponent();
        ViewModel = new NamedIconControlViewModel();
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
}
