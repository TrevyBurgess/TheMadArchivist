using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.IO;

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
    public void LoadFromJsonFile_FileMissing_DoesNotThrow_AndLeavesItemsEmpty()
    {
        var vm = new NamedIconControlViewModel(
            fileExists: _ => false,
            readAllText: _ => throw new InvalidOperationException("Should not read"));

        vm.LoadFromJsonFile("C:\\does-not-exist.json");

        Assert.AreEqual(0, vm.Items.Count);
        Assert.IsFalse(vm.IsSaveEnabled);
    }

    [TestMethod]
    public void LoadFromJsonFile_ValidJson_LoadsRows_AndIsNotDirty()
    {
        var json = "{\"items\":[{\"iconPath\":\"C:/icons/a.png\",\"text\":\"Alpha\"},{\"iconPath\":\"C:/icons/b.png\",\"text\":\"Beta\"}]}";
        var vm = new NamedIconControlViewModel(
            fileExists: _ => true,
            readAllText: _ => json);

        vm.LoadFromJsonFile("C:\\data.json");

        Assert.AreEqual(2, vm.Items.Count);
        Assert.AreEqual("C:/icons/a.png", vm.Items[0].IconPath);
        Assert.AreEqual("Alpha", vm.Items[0].Text);
        Assert.IsFalse(vm.IsSaveEnabled);
    }

    [TestMethod]
    public void EditingRowText_MarksDirty()
    {
        var json = "{\"items\":[{\"iconPath\":\"C:/icons/a.png\",\"text\":\"Alpha\"}]}";
        var vm = new NamedIconControlViewModel(
            fileExists: _ => true,
            readAllText: _ => json);

        vm.LoadFromJsonFile("C:\\data.json");
        Assert.IsFalse(vm.IsSaveEnabled);

        vm.Items[0].Text = "Alpha2";
        Assert.IsTrue(vm.IsSaveEnabled);
    }

    [TestMethod]
    public void SaveToJsonFile_WritesIndentedJson_AndClearsDirty()
    {
        string? writtenPath = null;
        string? writtenContent = null;
        string? createdDir = null;

        var vm = new NamedIconControlViewModel(
            createDirectory: d => createdDir = d,
            writeAllText: (p, c) =>
            {
                writtenPath = p;
                writtenContent = c;
            });

        vm.Items.Add(new NamedIconRowViewModel("C:/icons/a.png", "Alpha"));
        Assert.IsTrue(vm.IsSaveEnabled);

        vm.SaveToJsonFile("C:\\Temp\\NamedIconControl.json");

        Assert.AreEqual("C:\\Temp", createdDir);
        Assert.AreEqual("C:\\Temp\\NamedIconControl.json", writtenPath);
        Assert.IsNotNull(writtenContent);
        Assert.IsTrue(writtenContent!.Contains("\"items\"", StringComparison.OrdinalIgnoreCase));
        Assert.IsTrue(writtenContent.Contains("\"iconPath\"", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(vm.IsSaveEnabled);
    }

    [TestMethod]
    public void SaveToProgramData_UsesCommonAppDataFolder()
    {
        var programData = Path.Combine(Path.GetTempPath(), "NamedIconControlTests", Guid.NewGuid().ToString("N"));

        string? finalWrittenPath = null;
        var vm = new NamedIconControlViewModel(
            getProgramDataFolder: () => programData,
            createDirectory: _ => { },
            writeAllText: (p, _) => finalWrittenPath = p);

        vm.Items.Add(new NamedIconRowViewModel("C:/icons/a.png", "Alpha"));
        vm.SaveToProgramData();

        Assert.AreEqual(Path.Combine(programData, "TheMadArchivist", "NamedIconControl.json"), finalWrittenPath);
    }
}
