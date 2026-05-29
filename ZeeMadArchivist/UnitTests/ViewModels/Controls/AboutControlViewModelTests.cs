using CyberFeedForward.TheMadArchivist.Models;
using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ViewModels.Controls;

[TestClass]
public sealed class AboutControlViewModelTests
{
    [TestMethod]
    public void Ctor_WhenNoModelProvided_UsesDefaultModel()
    {
        var vm = new AboutControlViewModel();

        Assert.IsNotNull(vm.Model);
        Assert.IsNotNull(vm.Model.ApplicationName);
        Assert.IsNotNull(vm.Model.Version);
        Assert.IsNotNull(vm.Model.Description);
    }

    [TestMethod]
    public void Model_WhenSet_RaisesPropertyChanged()
    {
        var raised = false;
        var vm = new AboutControlViewModel(new AboutModel
        {
            ApplicationName = "App",
            Version = "Version 1",
            Description = "Desc",
        });

        vm.PropertyChanged += (_, e) =>
        {
            if (string.Equals(e.PropertyName, nameof(AboutControlViewModel.Model), System.StringComparison.Ordinal))
            {
                raised = true;
            }
        };

        vm.Model = new AboutModel
        {
            ApplicationName = "App2",
            Version = "Version 2",
            Description = "Desc2",
        };

        Assert.IsTrue(raised);
    }
}
