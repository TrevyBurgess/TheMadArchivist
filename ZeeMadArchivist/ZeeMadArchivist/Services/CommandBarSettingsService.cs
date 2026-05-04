namespace CyberFeedForward.TheMadArchivist.Services;

public sealed class CommandBarSettingsService
{
    private const string CommandBarOnLeftKey = "Layout.CommandBarOnLeft";
    private readonly IAppSettingsStore _store;

    public CommandBarSettingsService(IAppSettingsStore store)
    {
        _store = store;
    }

    public bool IsCommandBarOnLeft()
    {
        return !_store.TryGetBool(CommandBarOnLeftKey, out var value) || value;
    }

    public void SetCommandBarOnLeft(bool onLeft)
    {
        _store.SetBool(CommandBarOnLeftKey, onLeft);
    }
}
