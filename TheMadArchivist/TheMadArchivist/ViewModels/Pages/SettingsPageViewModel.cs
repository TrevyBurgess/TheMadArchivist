using CyberFeedForward.TheMadArchivist.Services;
using CyberFeedForward.TheMadArchivist.Utilities;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CyberFeedForward.TheMadArchivist;

namespace CyberFeedForward.TheMadArchivist.ViewModels.Pages;

public sealed class SettingsPageViewModel : INotifyPropertyChanged
{
    private readonly ThemeSettingsService _themeSettingsService;
    private readonly CommandBarSettingsService _commandBarSettingsService;
    private readonly ArchivesSettingsService _archivesSettingsService;
    private readonly FrameworkElement? _themeRootElement;
    private AppThemeMode _themeMode;
    private bool _isCommandBarOnLeft;
    private string _newArchivePath = string.Empty;
    private string? _selectedArchive;

    public SettingsPageViewModel()
        : this(
            new ThemeSettingsService(new LocalAppSettingsStore()),
            new CommandBarSettingsService(new LocalAppSettingsStore()),
            new ArchivesSettingsService(new LocalAppSettingsStore()),
            App.MainWindowInstance?.Content as FrameworkElement)
    {
    }

    public SettingsPageViewModel(
        ThemeSettingsService themeSettingsService,
        CommandBarSettingsService commandBarSettingsService,
        ArchivesSettingsService archivesSettingsService,
        FrameworkElement? themeRootElement)
    {
        _themeSettingsService = themeSettingsService;
        _commandBarSettingsService = commandBarSettingsService;
        _archivesSettingsService = archivesSettingsService;
        _themeRootElement = themeRootElement;

        _themeMode = _themeSettingsService.GetThemeMode();
        _isCommandBarOnLeft = _commandBarSettingsService.IsCommandBarOnLeft();

        Archives = new ObservableCollection<string>(_archivesSettingsService.GetArchives());
        Archives.CollectionChanged += Archives_OnCollectionChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public AppThemeMode ThemeMode
    {
        get => _themeMode;
        set
        {
            if (_themeMode == value)
            {
                return;
            }

            _themeMode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsDarkModeEnabled));

            _themeSettingsService.SetThemeMode(value);

            if (_themeRootElement is not null)
            {
                AppThemeManager.ApplyThemeMode(_themeRootElement, value);
            }
        }
    }

    public ObservableCollection<string> Archives { get; }

    public string NewArchivePath
    {
        get => _newArchivePath;
        set
        {
            var next = value ?? string.Empty;
            if (string.Equals(_newArchivePath, next, System.StringComparison.Ordinal))
            {
                return;
            }

            _newArchivePath = next;
            OnPropertyChanged();
        }
    }

    public string? SelectedArchive
    {
        get => _selectedArchive;
        set
        {
            if (string.Equals(_selectedArchive, value, System.StringComparison.Ordinal))
            {
                return;
            }

            _selectedArchive = value;
            OnPropertyChanged();
        }
    }

    public void AddArchive()
    {
        var next = NewArchivePath?.Trim();
        if (string.IsNullOrWhiteSpace(next))
        {
            return;
        }

        if (Archives.Any(a => string.Equals(a, next, System.StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        Archives.Add(next);
        NewArchivePath = string.Empty;
    }

    public void RemoveSelectedArchive()
    {
        if (string.IsNullOrWhiteSpace(SelectedArchive))
        {
            return;
        }

        var toRemove = Archives.FirstOrDefault(a => string.Equals(a, SelectedArchive, System.StringComparison.OrdinalIgnoreCase));
        if (toRemove is null)
        {
            return;
        }

        Archives.Remove(toRemove);
        SelectedArchive = null;
    }

    private void Archives_OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _archivesSettingsService.SaveArchives(Archives);
    }

    public bool IsDarkModeEnabled
    {
        get => ThemeMode == AppThemeMode.Dark;
        set => ThemeMode = value ? AppThemeMode.Dark : AppThemeMode.Light;
    }

    public int ThemeModeIndex
    {
        get => ThemeMode switch
        {
            AppThemeMode.SystemDefault => 0,
            AppThemeMode.Light => 1,
            _ => 2,
        };
        set => ThemeMode = value switch
        {
            1 => AppThemeMode.Light,
            2 => AppThemeMode.Dark,
            _ => AppThemeMode.SystemDefault,
        };
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

            if (App.MainWindowInstance is MainWindow mainWindow)
            {
                mainWindow.SetCommandBarOnLeft(value);
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
