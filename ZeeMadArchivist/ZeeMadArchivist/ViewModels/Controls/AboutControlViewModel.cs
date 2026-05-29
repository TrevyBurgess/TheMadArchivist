using CyberFeedForward.TheMadArchivist.Models;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CyberFeedForward.TheMadArchivist.ViewModels.Controls;

public sealed class AboutControlViewModel : INotifyPropertyChanged
{
    private AboutModel _model;

    public AboutControlViewModel(AboutModel? model = null)
    {
        _model = model ?? AboutModel.CreateDefault();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public AboutModel Model
    {
        get => _model;
        set
        {
            if (ReferenceEquals(_model, value))
            {
                return;
            }

            _model = value ?? throw new ArgumentNullException(nameof(value));
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
