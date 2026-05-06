using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.UI.Xaml.Media.Imaging;

namespace CyberFeedForward.TheMadArchivist.ViewModels.Controls;

public sealed class NamedIconControlViewModel : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private Symbol _iconSymbol = Symbol.Placeholder;
    private readonly Func<string> _getProgramDataFolder;
    private readonly Func<string, bool> _fileExists;
    private readonly Func<string, string> _readAllText;
    private readonly Action<string> _createDirectory;
    private readonly Action<string, string> _writeAllText;
    private bool _isSaveEnabled;
    private bool _suppressDirtyTracking;

    private const string DefaultFolderName = "TheMadArchivist";
    private const string DefaultFileName = "NamedIconControl.json";

    public NamedIconControlViewModel(
        Func<string>? getProgramDataFolder = null,
        Func<string, bool>? fileExists = null,
        Func<string, string>? readAllText = null,
        Action<string>? createDirectory = null,
        Action<string, string>? writeAllText = null)
    {
        _getProgramDataFolder = getProgramDataFolder ?? (() => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
        _fileExists = fileExists ?? File.Exists;
        _readAllText = readAllText ?? File.ReadAllText;
        _createDirectory = createDirectory ?? (p => Directory.CreateDirectory(p));
        _writeAllText = writeAllText ?? File.WriteAllText;

        Items = new ObservableCollection<NamedIconRowViewModel>();
        Items.CollectionChanged += (_, __) =>
        {
            if (_suppressDirtyTracking)
            {
                return;
            }

            IsSaveEnabled = true;
        };
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
        var folder = BuildDataFolderPath(folderName);
        var path = Path.Combine(folder, fileName ?? DefaultFileName);
        LoadFromJsonFile(path);
    }

    public void SaveToProgramData(string? folderName = null, string? fileName = null)
    {
        var folder = BuildDataFolderPath(folderName);
        var path = Path.Combine(folder, fileName ?? DefaultFileName);
        SaveToJsonFile(path);
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
            parsed = JsonSerializer.Deserialize<NamedIconControlFileModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
            Items = Items
                .Select(i => new NamedIconRowFileModel
                {
                    IconPath = i.IconPath,
                    Text = i.Text,
                })
                .ToList(),
        };

        var json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
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
}

public sealed class NamedIconRowViewModel : INotifyPropertyChanged
{
    private string _iconPath;
    private string _text;

    public NamedIconRowViewModel(string? iconPath, string? text)
    {
        _iconPath = iconPath ?? string.Empty;
        _text = text ?? string.Empty;
    }

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
