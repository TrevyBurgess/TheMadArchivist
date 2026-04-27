using CyberFeedForward.TheMadArchivist.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests.Services;

[TestClass]
public sealed class ThemeSettingsServiceTests
{
    private sealed class InMemorySettingsStore : IAppSettingsStore
    {
        private readonly Dictionary<string, bool> _values = new();

        public bool TryGetBool(string key, out bool value)
        {
            return _values.TryGetValue(key, out value);
        }

        public void SetBool(string key, bool value)
        {
            _values[key] = value;
        }
    }

    [TestMethod]
    public void IsDarkModeEnabled_WhenNotSet_ReturnsFalse()
    {
        var store = new InMemorySettingsStore();
        var service = new ThemeSettingsService(store);

        Assert.IsFalse(service.IsDarkModeEnabled());
    }

    [TestMethod]
    public void SetDarkModeEnabled_WhenSetToTrue_PersistsValue()
    {
        var store = new InMemorySettingsStore();
        var service = new ThemeSettingsService(store);

        service.SetDarkModeEnabled(true);

        Assert.IsTrue(service.IsDarkModeEnabled());
    }

    [TestMethod]
    public void SetDarkModeEnabled_WhenSetToFalse_PersistsValue()
    {
        var store = new InMemorySettingsStore();
        var service = new ThemeSettingsService(store);

        service.SetDarkModeEnabled(true);
        service.SetDarkModeEnabled(false);

        Assert.IsFalse(service.IsDarkModeEnabled());
    }
}
