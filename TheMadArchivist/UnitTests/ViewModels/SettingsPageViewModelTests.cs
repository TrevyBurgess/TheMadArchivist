using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Collections.Generic;
using TheMadArchivist.Services;
using TheMadArchivist.ViewModels;

namespace UnitTests.ViewModels;

[TestClass]
public sealed class SettingsPageViewModelTests
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

    [UITestMethod]
    public void IsDarkModeEnabled_WhenSet_UpdatesRootElementTheme()
    {
        var store = new InMemorySettingsStore();
        var service = new ThemeSettingsService(store);
        var root = new Grid();

        var viewModel = new SettingsPageViewModel(service, root);

        viewModel.IsDarkModeEnabled = true;

        Assert.AreEqual(ElementTheme.Dark, root.RequestedTheme);
    }
}
