using CyberFeedForward.TheMadArchivist.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTests.Services;

[TestClass]
public sealed class TrayIconServiceTests
{
    private sealed class FakePopupMenu : TrayIconService.ITrayPopupMenu
    {
        public IntPtr Handle => new IntPtr(123);

        public void AppendItem(uint commandId, string text)
        {
        }

        public void Dispose()
        {
        }
    }

    private sealed class FakeNative : TrayIconService.ITrayIconNative
    {
        public bool AddCalled { get; private set; }
        public bool RemoveCalled { get; private set; }

        public IntPtr CreateMessageWindow(Delegate wndProc)
        {
            return new IntPtr(42);
        }

        public void DestroyMessageWindow(IntPtr hwnd)
        {
        }

        public IntPtr LoadIconFromFile(string path)
        {
            return new IntPtr(77);
        }

        public void DestroyIcon(IntPtr hIcon)
        {
        }

        public bool AddNotifyIcon(IntPtr hwnd, uint iconId, uint callbackMessage, IntPtr hIcon, string tooltip)
        {
            AddCalled = true;
            return true;
        }

        public bool RemoveNotifyIcon(IntPtr hwnd, uint iconId)
        {
            RemoveCalled = true;
            return true;
        }

        public IntPtr DefWindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return IntPtr.Zero;
        }

        public bool TryGetCursorPos(out TrayIconService.POINT pt)
        {
            pt = new TrayIconService.POINT { X = 1, Y = 2 };
            return true;
        }

        public TrayIconService.ITrayPopupMenu CreatePopupMenu()
        {
            return new FakePopupMenu();
        }

        public void TrackPopupMenu(IntPtr hMenu, int x, int y, IntPtr hwnd)
        {
        }

        public void SetForegroundWindow(IntPtr hwnd)
        {
        }
    }

    [TestMethod]
    public void Initialize_WhenCalledMultipleTimes_DoesNotThrow()
    {
        var tempFolder = Path.Combine(Path.GetTempPath(), $"TheMadArchivist_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempFolder);

        try
        {
            var native = new FakeNative();
            var service = new TrayIconService(
                getIconFilePath: () => Path.Combine(tempFolder, "missing.ico"),
                native: native);

            service.Initialize();
            service.Initialize();

            Assert.IsTrue(service.IsInitialized);
            Assert.IsTrue(native.AddCalled);

            service.Dispose();
            service.Dispose();

            Assert.IsTrue(native.RemoveCalled);
        }
        finally
        {
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, recursive: true);
            }
        }
    }
}
