using CyberFeedForward.TheMadArchivist.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ViewModels;

[TestClass]
public sealed class HomePageViewModelTests
{
    [TestMethod]
    public void Title_IsNotEmpty()
    {
        var vm = new HomePageViewModel();
        Assert.IsFalse(string.IsNullOrWhiteSpace(vm.Title));
    }

    [TestMethod]
    public void Description_IsNotEmpty()
    {
        var vm = new HomePageViewModel();
        Assert.IsFalse(string.IsNullOrWhiteSpace(vm.Description));
    }
}
