namespace TheMadArchivist.Services;

public sealed class ThemeSettingsService
{
    private const string DarkModeEnabledKey = "Theme.DarkModeEnabled";
    private readonly IAppSettingsStore _store;

    public ThemeSettingsService(IAppSettingsStore store)
    {
        _store = store;
    }

    public bool IsDarkModeEnabled()
    {
        return _store.TryGetBool(DarkModeEnabledKey, out var value) && value;
    }

    public void SetDarkModeEnabled(bool enabled)
    {
        _store.SetBool(DarkModeEnabledKey, enabled);
    }
}
