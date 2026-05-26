using CyberFeedForward.TheMadArchivist.ViewModels.Controls;
using CyberFeedForward.TheMadArchivist.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.IO;

namespace UnitTests.ViewModels.Controls;

[TestClass]
public sealed class NamedIconControlViewModelTests
{
    private sealed class FakeAppSettingsStore : IAppSettingsStore
    {
        private readonly System.Collections.Generic.Dictionary<string, object?> _values = new(StringComparer.Ordinal);

        public bool TryGetBool(string key, out bool value)
        {
            value = false;
            return false;
        }

        public void SetBool(string key, bool value)
        {
        }

        public bool TryGetInt(string key, out int value)
        {
            value = 0;
            return false;
        }

        public void SetInt(string key, int value)
        {
        }

        public bool TryGetString(string key, out string value)
        {
            if (_values.TryGetValue(key, out var stored) && stored is string s)
            {
                value = s;
                return true;
            }

            value = string.Empty;
            return false;
        }

        public void SetString(string key, string value)
        {
            _values[key] = value;
        }
    }

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

    [TestMethod]
    public void LoadFromProgramData_DefaultsCustomIconsToDocumentsSubfolder_AndCreatesFolder()
    {
        var docs = "C:\\Users\\Test\\Documents";
        var expected = Path.Combine(docs, "ZeeMadArchivist", "CustomIcons");

        var store = new FakeAppSettingsStore();
        var settings = new CustomIconsSettingsService(store);

        string? createdPath = null;
        var vm = new NamedIconControlViewModel(
            customIconsSettingsService: settings,
            getDocumentsFolder: () => docs,
            directoryExists: _ => false,
            createDirectory: p => createdPath = p,
            fileExists: _ => false);

        vm.LoadFromProgramData();

        Assert.AreEqual(expected, vm.CustomIconsFolderPath);
        Assert.AreEqual(expected, createdPath);
    }

    [TestMethod]
    public void SettingCustomIconsFolderPath_EnablesSave_AndSavePersistsToSettings_AndCreatesFolderIfMissing()
    {
        var store = new FakeAppSettingsStore();
        var settings = new CustomIconsSettingsService(store);

        var created = 0;
        var vm = new NamedIconControlViewModel(
            customIconsSettingsService: settings,
            directoryExists: _ => false,
            createDirectory: _ => created++,
            fileExists: _ => false)
        {
            CustomIconsFolderPath = "C:\\Temp\\CustomIcons"
        };

        Assert.AreEqual("C:\\Temp\\CustomIcons", vm.CustomIconsFolderPath);
        Assert.IsTrue(vm.IsCustomIconsPathSaveEnabled);

        vm.SaveCustomIconsFolderPath();

        Assert.AreEqual(1, created);
        Assert.AreEqual("C:\\Temp\\CustomIcons", settings.GetCustomIconsFolderPath());
        Assert.IsFalse(vm.IsCustomIconsPathSaveEnabled);
    }

    [TestMethod]
    public void IconList_WhenCustomIconsDirectoryExists_IsPopulatedAndSortedByFileName()
    {
        var store = new FakeAppSettingsStore();
        var settings = new CustomIconsSettingsService(store);

        var vm = new NamedIconControlViewModel(
            customIconsSettingsService: settings,
            directoryExists: _ => true,
            enumerateFiles: _ =>
            [
                "C:\\Icons\\b.png",
                "C:\\Icons\\a.png",
            ],
            fileExists: _ => false)
        {
            CustomIconsFolderPath = "C:\\Icons"
        };

        Assert.AreEqual(2, vm.IconList.Count);
        Assert.AreEqual("a", vm.IconList[0].Name);
        Assert.AreEqual("b", vm.IconList[1].Name);
    }

    [TestMethod]
    public void IconList_WhenCustomIconsDirectoryMissing_IsEmpty()
    {
        var store = new FakeAppSettingsStore();
        var settings = new CustomIconsSettingsService(store);

        var vm = new NamedIconControlViewModel(
            customIconsSettingsService: settings,
            directoryExists: _ => false,
            enumerateFiles: _ => ["C:\\Icons\\a.png"],
            fileExists: _ => false)
        {
            CustomIconsFolderPath = "C:\\Icons"
        };

        Assert.AreEqual(0, vm.IconList.Count);
    }

    [TestMethod]
    public void SettingCustomIconsFolderPath_WhenDirectoryMissing_CreatesDirectory()
    {
        var store = new FakeAppSettingsStore();
        var settings = new CustomIconsSettingsService(store);

        var created = 0;
        var vm = new NamedIconControlViewModel(
            customIconsSettingsService: settings,
            directoryExists: _ => false,
            createDirectory: _ => created++,
            enumerateFiles: _ => [],
            fileExists: _ => false);

        vm.CustomIconsFolderPath = "C:\\Icons";

        Assert.AreEqual(1, created);
    }

    [TestMethod]
    public void RefreshIcons_WhenDirectoryMissing_CreatesDirectory()
    {
        var store = new FakeAppSettingsStore();
        var settings = new CustomIconsSettingsService(store);

        var created = 0;
        var vm = new NamedIconControlViewModel(
            customIconsSettingsService: settings,
            directoryExists: _ => false,
            createDirectory: _ => created++,
            enumerateFiles: _ => [],
            fileExists: _ => false)
        {
            CustomIconsFolderPath = "C:\\Icons"
        };

        Assert.AreEqual(1, created);

        vm.RefreshIcons();

        Assert.AreEqual(2, created);
    }
}
