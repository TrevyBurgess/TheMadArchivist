using CyberFeedForward.TheMadArchivist.Services;
using CyberFeedForward.TheMadArchivist.Utilities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberFeedForward.TheMadArchivist.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly CommandBarSettingsService _commandBarSettingsService;
    private string _statusText;
    private bool _isCommandBarOnLeft;

    public MainWindowViewModel()
        : this(
            new CommandBarSettingsService(new LocalAppSettingsStore()),
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
    {
    }

    public MainWindowViewModel(CommandBarSettingsService commandBarSettingsService, string defaultFolderPath)
    {
        _commandBarSettingsService = commandBarSettingsService;

        _statusText = "Ready";
        _isCommandBarOnLeft = _commandBarSettingsService.IsCommandBarOnLeft();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string StatusText
    {
        get => _statusText;
        set
        {
            var next = string.IsNullOrWhiteSpace(value) ? "Ready" : value;
            if (string.Equals(_statusText, next, StringComparison.Ordinal))
            {
                return;
            }

            _statusText = next;
            OnPropertyChanged();
        }
    }

    public bool IsCommandBarOnLeft
    {
        get => _isCommandBarOnLeft;
        set
        {
            if (_isCommandBarOnLeft == value)
            {
                return;
            }

            _isCommandBarOnLeft = value;
            OnPropertyChanged();

            _commandBarSettingsService.SetCommandBarOnLeft(value);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
