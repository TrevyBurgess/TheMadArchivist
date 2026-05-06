using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UnitTests.ViewModels.Controls;

[TestClass]
public sealed class NamedIconControlViewModelTests
{
    [TestMethod]
    public void PropertyChanged_HasExpectedHandlerType()
    {
        var vm = new NamedIconControlViewModel();
        Assert.IsNotNull(vm as INotifyPropertyChanged);
    }

    [TestMethod]
    public void SettingName_ToNull_NormalizesToEmpty_AndFiresPropertyChanged()
    {
        var vm = new NamedIconControlViewModel();
        vm.Name = "Before";

        var changed = new List<string?>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.Name = null!;

        Assert.AreEqual(string.Empty, vm.Name);
        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual(nameof(NamedIconControlViewModel.Name), changed[0]);
    }

    [TestMethod]
    public void SettingName_ToSameValue_DoesNotFirePropertyChanged()
    {
        var vm = new NamedIconControlViewModel();
        vm.Name = "Test";

        var eventCount = 0;
        vm.PropertyChanged += (_, e) =>
        {
            if (string.Equals(e.PropertyName, nameof(NamedIconControlViewModel.Name), StringComparison.Ordinal))
            {
                eventCount++;
            }
        };

        vm.Name = "Test";

        Assert.AreEqual(0, eventCount);
    }

    [TestMethod]
    public void SettingIconSymbol_ToDifferentValue_FiresPropertyChanged()
    {
        var vm = new NamedIconControlViewModel();

        var changed = new List<string?>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.IconSymbol = Symbol.Folder;

        Assert.AreEqual(Symbol.Folder, vm.IconSymbol);
        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual(nameof(NamedIconControlViewModel.IconSymbol), changed[0]);
    }

    [TestMethod]
    public void SettingIconSymbol_ToSameValue_DoesNotFirePropertyChanged()
    {
        var vm = new NamedIconControlViewModel();
        vm.IconSymbol = Symbol.Folder;

        var eventCount = 0;
        vm.PropertyChanged += (_, e) =>
        {
            if (string.Equals(e.PropertyName, nameof(NamedIconControlViewModel.IconSymbol), StringComparison.Ordinal))
            {
                eventCount++;
            }
        };

        vm.IconSymbol = Symbol.Folder;

        Assert.AreEqual(0, eventCount);
    }
}
