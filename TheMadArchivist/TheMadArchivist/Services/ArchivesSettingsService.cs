using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CyberFeedForward.TheMadArchivist.Services;

public sealed class ArchivesSettingsService
{
    private const string ArchivesKey = "Archives.Paths";
    private readonly IAppSettingsStore _store;

    public ArchivesSettingsService(IAppSettingsStore store)
    {
        _store = store;
    }

    public IReadOnlyList<string> GetArchives()
    {
        if (!_store.TryGetString(ArchivesKey, out var json) || string.IsNullOrWhiteSpace(json))
        {
            return Array.Empty<string>();
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<List<string>>(json);
            if (parsed is null)
            {
                return Array.Empty<string>();
            }

            return parsed
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    public void SaveArchives(IEnumerable<string> archives)
    {
        if (archives is null)
        {
            throw new ArgumentNullException(nameof(archives));
        }

        var cleaned = archives
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var json = JsonSerializer.Serialize(cleaned);
        _store.SetString(ArchivesKey, json);
    }
}
