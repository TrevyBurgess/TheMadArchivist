using CyberFeedForward.TheMadArchivist.Services;
using CyberFeedForward.TheMadArchivist.ViewModels.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ViewModels.Pages;

[TestClass]
public sealed class SettingsPageViewModelTests
{
    private sealed class FakeAppSettingsStore : IAppSettingsStore
    {
        public bool TryGetBool(string key, out bool value)
        {
            value = false;
            return false;
        }

        public void SetBool(string key, bool value)
        {
        }

        public bool TryGetInt(string key, out int value)
        {
            value = 0;
            return false;
        }

        public void SetInt(string key, int value)
        {
        }

        public bool TryGetString(string key, out string value)
        {
            value = string.Empty;
            return false;
        }

        public void SetString(string key, string value)
        {
        }
    }

    [TestMethod]
    public void TrySetStartupEnabled_WhenServiceSucceeds_ReturnsTrueAndUpdatesSetStartup()
    {
        var theme = new ThemeSettingsService(new FakeAppSettingsStore());
        var cmd = new CommandBarSettingsService(new FakeAppSettingsStore());

        var startup = new StartupSettingsService(
            getExecutablePath: () => "C:\\App\\ZeeMadArchivist.exe",
            tryReadRunValue: () => (null, false),
            writeRunValue: _ => { },
            deleteRunValue: () => { });

        var vm = new SettingsPageViewModel(theme, cmd, startup, themeRootElement: null);

        var ok = vm.TrySetStartupEnabled(true, out var errorMessage);

        Assert.IsTrue(ok);
        Assert.IsTrue(string.IsNullOrWhiteSpace(errorMessage));
    }

    [TestMethod]
    public void TrySetStartupEnabled_WhenServiceThrows_ReturnsFalseAndProvidesMessage()
    {
        var theme = new ThemeSettingsService(new FakeAppSettingsStore());
        var cmd = new CommandBarSettingsService(new FakeAppSettingsStore());

        var startup = new StartupSettingsService(
            getExecutablePath: () => "C:\\App\\ZeeMadArchivist.exe",
            tryReadRunValue: () => (null, false),
            writeRunValue: _ => throw new System.InvalidOperationException("no access"),
            deleteRunValue: () => { });

        var vm = new SettingsPageViewModel(theme, cmd, startup, themeRootElement: null);

        var ok = vm.TrySetStartupEnabled(true, out var errorMessage);

        Assert.IsFalse(ok);
        Assert.IsTrue(errorMessage?.Length > 0);
    }
}
