using CyberFeedForward.TheMadArchivist.Models;
using CyberFeedForward.TheMadArchivist.Services;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace CyberFeedForward.TheMadArchivist.Views.Controls;

public sealed partial class FolderContentsControl : UserControl
{
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly IFileSystemService _fileSystemService;
    private CancellationTokenSource? _loadCts;

    public FolderContentsControl()
    {
        InitializeComponent();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _fileSystemService = new FileSystemService();

        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = null;
    }

    public ObservableCollection<FileSystemEntry> Entries { get; } = new();

    public string? FolderPath
    {
        get => (string?)GetValue(FolderPathProperty);
        set => SetValue(FolderPathProperty, value);
    }

    public static readonly DependencyProperty FolderPathProperty =
        DependencyProperty.Register(
            nameof(FolderPath),
            typeof(string),
            typeof(FolderContentsControl),
            new PropertyMetadata(null, OnFolderPathChanged));

    private static void OnFolderPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (FolderContentsControl)d;
        control.LoadEntriesAsync((string?)e.NewValue);
    }

    private void LoadEntriesAsync(string? folderPath)
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();
        var ct = _loadCts.Token;

        _ = Task.Run(() =>
        {
            var items = _fileSystemService.GetEntries(folderPath ?? string.Empty);

            if (ct.IsCancellationRequested)
            {
                return;
            }

            _dispatcherQueue.TryEnqueue(() =>
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                Entries.Clear();
                foreach (var item in items)
                {
                    Entries.Add(item);
                }
            });
        }, ct);
    }
}
