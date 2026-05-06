using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberFeedForward.TheMadArchivist.ViewModels.Controls;

public sealed class NamedIconControlViewModel : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private Symbol _iconSymbol = Symbol.Placeholder;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set
        {
            var next = value ?? string.Empty;
            if (string.Equals(_name, next, StringComparison.Ordinal))
            {
                return;
            }

            _name = next;
            OnPropertyChanged();
        }
    }

    public Symbol IconSymbol
    {
        get => _iconSymbol;
        set
        {
            if (_iconSymbol == value)
            {
                return;
            }

            _iconSymbol = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
