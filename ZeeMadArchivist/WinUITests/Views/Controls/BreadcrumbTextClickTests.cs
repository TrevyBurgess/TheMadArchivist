using CyberFeedForward.TheMadArchivist.Views.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace UnitTests.Views.Controls;

[TestClass]
public sealed class BreadcrumbTextClickTests
{
    [TestMethod]
    public void ClickingBreadcrumbText_RaisesFolderPathSelected_WithFolderPath()
    {
        WinUiTestHelper.Run(() =>
        {
            var control = new Breadcrumb
            {
                FolderPath = "C:\\Root\\Child",
            };

            string? selected = null;
            control.FolderPathSelected += (_, path) => selected = path;

            var textButton = (Button)control.FindName("BreadcrumbTextButton");

            var clickMethod = typeof(Breadcrumb).GetMethod(
                "BreadcrumbTextButton_OnClick",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(clickMethod);

            clickMethod.Invoke(control, [textButton, new RoutedEventArgs()]);

            Assert.AreEqual("C:\\Root\\Child", selected);
        });
    }
}
