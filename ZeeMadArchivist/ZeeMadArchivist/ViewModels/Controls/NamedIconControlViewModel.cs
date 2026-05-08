using CyberFeedForward.TheMadArchivist.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace CyberFeedForward.TheMadArchivist.ViewModels.Controls;

public sealed partial class NamedIconControlViewModel : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private Symbol _iconSymbol = Symbol.Placeholder;
    private string _customIconsFolderPath = string.Empty;
    private string _savedCustomIconsFolderPath = string.Empty;
    private readonly Func<string> _getProgramDataFolder;
    private readonly Func<string, bool> _fileExists;
    private readonly Func<string, string> _readAllText;
    private readonly Action<string> _createDirectory;
    private readonly Action<string, string> _writeAllText;
    private readonly Func<string, bool> _directoryExists;
    private readonly Func<string> _getDocumentsFolder;
    private readonly CustomIconsSettingsService _customIconsSettingsService;
    private bool _isSaveEnabled;
    private bool _suppressDirtyTracking;

    private const string DefaultFolderName = "TheMadArchivist";
    private const string DefaultFileName = "NamedIconControl.json";

    private static readonly JsonSerializerOptions DeserializeOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly JsonSerializerOptions SerializeOptions = new() { WriteIndented = true };

    public NamedIconControlViewModel(
        Func<string>? getProgramDataFolder = null,
        Func<string, bool>? fileExists = null,
        Func<string, string>? readAllText = null,
        Action<string>? createDirectory = null,
        Action<string, string>? writeAllText = null,
        Func<string, bool>? directoryExists = null,
        Func<string>? getDocumentsFolder = null,
        CustomIconsSettingsService? customIconsSettingsService = null)
    {
        _getProgramDataFolder = getProgramDataFolder ?? (() => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
        _fileExists = fileExists ?? File.Exists;
        _readAllText = readAllText ?? File.ReadAllText;
        _createDirectory = createDirectory ?? (p => Directory.CreateDirectory(p));
        _writeAllText = writeAllText ?? File.WriteAllText;
        _directoryExists = directoryExists ?? Directory.Exists;
        _getDocumentsFolder = getDocumentsFolder ?? (() => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        _customIconsSettingsService = customIconsSettingsService ?? CreateDefaultCustomIconsSettingsService();

        Items = [];
        Items.CollectionChanged += (_, __) =>
        {
            if (_suppressDirtyTracking)
            {
                return;
            }
            IsSaveEnabled = true;
        };
    }

    private static CustomIconsSettingsService CreateDefaultCustomIconsSettingsService()
    {
        try
        {
            return new CustomIconsSettingsService(new LocalAppSettingsStore());
        }
        catch
        {
            return new CustomIconsSettingsService(new InMemoryAppSettingsStore());
        }
    }

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

    public string CustomIconsFolderPath
    {
        get => _customIconsFolderPath;
        set
        {
            var normalized = NormalizeCustomIconsFolderPath(value);
            if (string.Equals(_customIconsFolderPath, normalized, StringComparison.Ordinal))
            {
                return;
            }

            _customIconsFolderPath = normalized;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCustomIconsPathSaveEnabled));
        }
    }

    public bool IsCustomIconsPathSaveEnabled
    {
        get
        {
            return !string.Equals(
                NormalizeCustomIconsFolderPath(_savedCustomIconsFolderPath),
                NormalizeCustomIconsFolderPath(CustomIconsFolderPath),
                StringComparison.Ordinal);
        }
    }

    public ObservableCollection<NamedIconRowViewModel> Items { get; }

    public bool IsSaveEnabled
    {
        get => _isSaveEnabled;
        private set
        {
            if (_isSaveEnabled == value)
            {
                return;
            }

            _isSaveEnabled = value;
            OnPropertyChanged();
        }
    }

    public void LoadFromProgramData(string? folderName = null, string? fileName = null)
    {
        LoadCustomIconsFolderFromSettings();
        var folder = BuildDataFolderPath(folderName);
        var path = Path.Combine(folder, fileName ?? DefaultFileName);
        LoadFromJsonFile(path);
    }

    public void SaveToProgramData(string? folderName = null, string? fileName = null)
    {
        EnsureCustomIconsFolderExists(CustomIconsFolderPath);
        var folder = BuildDataFolderPath(folderName);
        var path = Path.Combine(folder, fileName ?? DefaultFileName);
        SaveToJsonFile(path);
    }

    private void LoadCustomIconsFolderFromSettings()
    {
        var stored = _customIconsSettingsService.GetCustomIconsFolderPath();
        var normalized = NormalizeCustomIconsFolderPath(stored);

        _savedCustomIconsFolderPath = normalized;
        _customIconsFolderPath = normalized;
        EnsureCustomIconsFolderExists(_savedCustomIconsFolderPath);
        OnPropertyChanged(nameof(CustomIconsFolderPath));
        OnPropertyChanged(nameof(IsCustomIconsPathSaveEnabled));
    }

    public void SaveCustomIconsFolderPath()
    {
        var normalized = NormalizeCustomIconsFolderPath(CustomIconsFolderPath);
        _savedCustomIconsFolderPath = normalized;
        _customIconsFolderPath = normalized;

        _customIconsSettingsService.SetCustomIconsFolderPath(_savedCustomIconsFolderPath);
        EnsureCustomIconsFolderExists(_savedCustomIconsFolderPath);

        OnPropertyChanged(nameof(CustomIconsFolderPath));
        OnPropertyChanged(nameof(IsCustomIconsPathSaveEnabled));
    }

    private string NormalizeCustomIconsFolderPath(string? candidate)
    {
        var next = candidate?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(next))
        {
            return next;
        }

        var docs = _getDocumentsFolder();
        if (string.IsNullOrWhiteSpace(docs))
        {
            return "CustomIcons";
        }

        return Path.Combine(docs, "ZeeMadArchivist", "CustomIcons");
    }

    private void EnsureCustomIconsFolderExists(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            return;
        }

        try
        {
            if (_directoryExists(folderPath))
            {
                return;
            }

            _createDirectory(folderPath);
        }
        catch
        {
        }
    }

    private string BuildDataFolderPath(string? folderName)
    {
        var baseFolder = _getProgramDataFolder();
        if (string.IsNullOrWhiteSpace(baseFolder))
        {
            baseFolder = AppContext.BaseDirectory;
        }

        return Path.Combine(baseFolder, folderName ?? DefaultFolderName);
    }

    public void LoadFromJsonFile(string jsonFilePath)
    {
        if (string.IsNullOrWhiteSpace(jsonFilePath))
        {
            throw new ArgumentException("Path cannot be empty.", nameof(jsonFilePath));
        }

        _suppressDirtyTracking = true;
        Items.Clear();
        IsSaveEnabled = false;

        if (!_fileExists(jsonFilePath))
        {
            _suppressDirtyTracking = false;
            return;
        }

        string json;
        try
        {
            json = _readAllText(jsonFilePath);
        }
        catch
        {
            _suppressDirtyTracking = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            _suppressDirtyTracking = false;
            return;
        }

        NamedIconControlFileModel? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<NamedIconControlFileModel>(json, DeserializeOptions);
        }
        catch
        {
            _suppressDirtyTracking = false;
            return;
        }

        if (parsed?.Items is null)
        {
            _suppressDirtyTracking = false;
            return;
        }

        foreach (var i in parsed.Items.Where(i => i is not null))
        {
            var iconPath = i!.IconPath ?? string.Empty;
            var text = i.Text ?? string.Empty;

            var row = new NamedIconRowViewModel(iconPath, text);
            row.PropertyChanged += (_, __) =>
            {
                if (_suppressDirtyTracking)
                {
                    return;
                }

                IsSaveEnabled = true;
            };
            Items.Add(row);
        }

        _suppressDirtyTracking = false;
    }

    public void SaveToJsonFile(string jsonFilePath)
    {
        if (string.IsNullOrWhiteSpace(jsonFilePath))
        {
            throw new ArgumentException("Path cannot be empty.", nameof(jsonFilePath));
        }

        var folder = Path.GetDirectoryName(jsonFilePath);
        if (string.IsNullOrWhiteSpace(folder))
        {
            throw new ArgumentException("Path must include a directory.", nameof(jsonFilePath));
        }

        _createDirectory(folder);

        var model = new NamedIconControlFileModel
        {
            Items = [.. Items
                .Select(i => new NamedIconRowFileModel
                {
                    IconPath = i.IconPath,
                    Text = i.Text,
                })],
        };

        var json = JsonSerializer.Serialize(model, SerializeOptions);
        _writeAllText(jsonFilePath, json);
        IsSaveEnabled = false;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class NamedIconControlFileModel
    {
        public System.Collections.Generic.List<NamedIconRowFileModel>? Items { get; set; }
    }

    private sealed class NamedIconRowFileModel
    {
        public string? IconPath { get; set; }
        public string? Text { get; set; }
    }

    private sealed class InMemoryAppSettingsStore : IAppSettingsStore
    {
        private readonly System.Collections.Generic.Dictionary<string, object?> _values = new(StringComparer.Ordinal);

        public bool TryGetBool(string key, out bool value)
        {
            if (_values.TryGetValue(key, out var stored) && stored is bool b)
            {
                value = b;
                return true;
            }

            value = false;
            return false;
        }

        public void SetBool(string key, bool value)
        {
            _values[key] = value;
        }

        public bool TryGetInt(string key, out int value)
        {
            if (_values.TryGetValue(key, out var stored) && stored is int i)
            {
                value = i;
                return true;
            }

            value = 0;
            return false;
        }

        public void SetInt(string key, int value)
        {
            _values[key] = value;
        }

        public bool TryGetString(string key, out string value)
        {
            if (_values.TryGetValue(key, out var stored) && stored is string s)
            {
                value = s;
                return true;
            }

            value = string.Empty;
            return false;
        }

        public void SetString(string key, string value)
        {
            _values[key] = value;
        }
    }
}

public sealed partial class NamedIconRowViewModel(string? iconPath, string? text) : INotifyPropertyChanged
{
    private string _iconPath = iconPath ?? string.Empty;
    private string _text = text ?? string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string IconPath
    {
        get => _iconPath;
        set
        {
            var next = value ?? string.Empty;
            if (string.Equals(_iconPath, next, StringComparison.Ordinal))
            {
                return;
            }

            _iconPath = next;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IconUri));
            OnPropertyChanged(nameof(IconImage));
        }
    }

    public Uri? IconUri
    {
        get
        {
            if (string.IsNullOrWhiteSpace(IconPath))
            {
                return null;
            }

            try
            {
                return new Uri(IconPath, UriKind.RelativeOrAbsolute);
            }
            catch
            {
                return null;
            }
        }
    }

    public BitmapImage? IconImage
    {
        get
        {
            if (string.IsNullOrWhiteSpace(IconPath))
            {
                return null;
            }

            try
            {
                return new BitmapImage(new Uri(IconPath, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                return null;
            }
        }
    }

    public string Text
    {
        get => _text;
        set
        {
            var next = value ?? string.Empty;
            if (string.Equals(_text, next, StringComparison.Ordinal))
            {
                return;
            }

            _text = next;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
