using Windows.Storage;

namespace CyberFeedForward.TheMadArchivist.Services;

public sealed class LocalAppSettingsStore : IAppSettingsStore
{
    private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

    public bool TryGetBool(string key, out bool value)
    {
        if (_localSettings.Values.TryGetValue(key, out var stored) && stored is bool b)
        {
            value = b;
            return true;
        }

        value = false;
        return false;
    }

    public void SetBool(string key, bool value)
    {
        _localSettings.Values[key] = value;
    }
}
