using CyberFeedForward.TheMadArchivist.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests.Services;

[TestClass]
public sealed class CommandBarSettingsServiceTests
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
    public void IsCommandBarOnLeft_WhenNotSet_ReturnsTrue()
    {
        var store = new InMemorySettingsStore();
        var service = new CommandBarSettingsService(store);

        Assert.IsTrue(service.IsCommandBarOnLeft());
    }

    [TestMethod]
    public void SetCommandBarOnLeft_WhenSetToFalse_PersistsValue()
    {
        var store = new InMemorySettingsStore();
        var service = new CommandBarSettingsService(store);

        service.SetCommandBarOnLeft(false);

        Assert.IsFalse(service.IsCommandBarOnLeft());
    }

    [TestMethod]
    public void SetCommandBarOnLeft_WhenSetToTrue_PersistsValue()
    {
        var store = new InMemorySettingsStore();
        var service = new CommandBarSettingsService(store);

        service.SetCommandBarOnLeft(false);
        service.SetCommandBarOnLeft(true);

        Assert.IsTrue(service.IsCommandBarOnLeft());
    }
}
