using System;

namespace CyberFeedForward.TheMadArchivist.Services;

public sealed class BackgroundCloseWarningService
{
    private const string WarnedKey = "App.CloseToBackground.Warned";

    private readonly IAppSettingsStore _store;

    public BackgroundCloseWarningService(IAppSettingsStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public bool ShouldShowWarning()
    {
        return !_store.TryGetBool(WarnedKey, out var warned) || !warned;
    }

    public void MarkWarned()
    {
        _store.SetBool(WarnedKey, true);
    }
}
