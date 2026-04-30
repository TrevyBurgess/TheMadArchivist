using CyberFeedForward.TheMadArchivist.Services;
using CyberFeedForward.TheMadArchivist.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests.AA.ViewModels;

[TestClass]
public sealed class MainWindowViewModelTests
{
    private sealed class InMemorySettingsStore : IAppSettingsStore
    {
        private readonly Dictionary<string, bool> _boolValues = new();
        private readonly Dictionary<string, int> _intValues = new();

        public bool TryGetBool(string key, out bool value)
        {
            return _boolValues.TryGetValue(key, out value);
        }

        public void SetBool(string key, bool value)
        {
            _boolValues[key] = value;
        }

        public bool TryGetInt(string key, out int value)
        {
            return _intValues.TryGetValue(key, out value);
        }

        public void SetInt(string key, int value)
        {
            _intValues[key] = value;
        }
    }

    [TestMethod]
    public void StatusText_WhenNullOrWhitespace_DefaultsToReady()
    {
        var store = new InMemorySettingsStore();
        var service = new CommandBarSettingsService(store);
        var vm = new MainWindowViewModel(service, "C:\\");

        vm.StatusText = "";
        Assert.AreEqual("Ready", vm.StatusText);

        vm.StatusText = "   ";
        Assert.AreEqual("Ready", vm.StatusText);

        vm.StatusText = null!;
        Assert.AreEqual("Ready", vm.StatusText);
    }

    [TestMethod]
    public void IsCommandBarOnLeft_WhenSet_PersistsToStore()
    {
        var store = new InMemorySettingsStore();
        var service = new CommandBarSettingsService(store);
        var vm = new MainWindowViewModel(service, "C:\\");

        vm.IsCommandBarOnLeft = false;

        Assert.IsFalse(service.IsCommandBarOnLeft());
    }
}
