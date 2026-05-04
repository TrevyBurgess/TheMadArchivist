using CyberFeedForward.TheMadArchivist.ViewModels.Pages;
using CyberFeedForward.TheMadArchivist.Views.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Views.Controls;

[TestClass]
public sealed class BreadcrumbBarControlTests
{
    [TestMethod]
    public void FolderPathBinding_WhenSourceChanges_UpdatesControlFolderPath()
    {
        WinUiTestHelper.Run(() =>
        {
            var vm = new HomePageViewModel();
            var control = new BreadcrumbBar
            {
                DataContext = vm,
            };

            control.SetBinding(BreadcrumbBar.FolderPathProperty, new Binding
            {
                Path = new PropertyPath(nameof(HomePageViewModel.FolderPath)),
                Mode = BindingMode.OneWay,
            });

            vm.FolderPath = "C:\\Temp";

            Assert.AreEqual("C:\\Temp", control.FolderPath);
        });
    }
}
