using CyberFeedForward.TheMadArchivist.ViewModels.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel;

namespace UnitTests.ViewModels.Pages;

[TestClass]
public sealed class HomePageViewModelTests
{
    [TestMethod]
    public void SettingFolderPath_ToDifferentValue_FiresPropertyChanged()
    {
        var vm = new HomePageViewModel();

        var changed = new List<string?>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.FolderPath = "C:\\Temp";

        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual(nameof(HomePageViewModel.FolderPath), changed[0]);
    }

    [TestMethod]
    public void SettingFolderPath_ToSameValue_DoesNotFirePropertyChanged()
    {
        var vm = new HomePageViewModel();
        vm.FolderPath = "C:\\Temp";

        var eventCount = 0;
        vm.PropertyChanged += (_, e) =>
        {
            if (string.Equals(e.PropertyName, nameof(HomePageViewModel.FolderPath), System.StringComparison.Ordinal))
            {
                eventCount++;
            }
        };

        vm.FolderPath = "C:\\Temp";

        Assert.AreEqual(0, eventCount);
    }

    [TestMethod]
    public void PropertyChanged_HasExpectedHandlerType()
    {
        var vm = new HomePageViewModel();
        Assert.IsNotNull(vm as INotifyPropertyChanged);
    }
}
