using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CyberFeedForward.TheMadArchivist.Views.Controls;

public sealed partial class AboutControl : UserControl
{
    public AboutControl()
    {
        InitializeComponent();
        ViewModel = new AboutControlViewModel();
    }

    public AboutControlViewModel ViewModel
    {
        get => (AboutControlViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(AboutControlViewModel),
            typeof(AboutControl),
            new PropertyMetadata(null));
}
