namespace CyberFeedForward.TheMadArchivist.Services;

public interface IAppSettingsStore
{
    bool TryGetBool(string key, out bool value);
    void SetBool(string key, bool value);
}
