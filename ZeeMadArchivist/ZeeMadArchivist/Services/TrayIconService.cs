using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CyberFeedForward.TheMadArchivist.Services;

public sealed partial class TrayIconService(Func<string?>? getIconFilePath = null, TrayIconService.ITrayIconNative? native = null) : IDisposable
{
    private const uint CallbackMessage = WM_APP + 1;
    private const uint TrayIconId = 1;
    private const uint ExitCommandId = 100;

    private readonly Func<string?> _getIconFilePath = getIconFilePath ?? GetDefaultTrayIconPath;
    private readonly ITrayIconNative _native = native ?? new TrayIconNative();

    private IntPtr _hwnd;
    private IntPtr _hIcon;
    private WndProc? _wndProc;
    private bool _isInitialized;

    public bool IsInitialized => _isInitialized;

    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _wndProc = WindowProc;
        _hwnd = _native.CreateMessageWindow(_wndProc);
        if (_hwnd == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to create tray message window.");
        }

        var iconPath = _getIconFilePath();
        if (!string.IsNullOrWhiteSpace(iconPath) && File.Exists(iconPath))
        {
            _hIcon = _native.LoadIconFromFile(iconPath);
        }
        else
        {
            _hIcon = _native.LoadApplicationIcon();
        }

        var ok = _native.AddNotifyIcon(_hwnd, TrayIconId, CallbackMessage, _hIcon, "ZeeMadArchivist");
        if (!ok)
        {
            throw new InvalidOperationException("Failed to add tray icon.");
        }

        _isInitialized = true;
    }

    public void Dispose()
    {
        if (!_isInitialized)
        {
            return;
        }

        try
        {
            _native.RemoveNotifyIcon(_hwnd, TrayIconId);
        }
        finally
        {
            if (_hIcon != IntPtr.Zero)
            {
                _native.DestroyIcon(_hIcon);
                _hIcon = IntPtr.Zero;
            }

            if (_hwnd != IntPtr.Zero)
            {
                _native.DestroyMessageWindow(_hwnd);
                _hwnd = IntPtr.Zero;
            }

            _isInitialized = false;
        }
    }

    private IntPtr WindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == CallbackMessage)
        {
            var eventId = (uint)lParam.ToInt64();
            if (eventId == WM_RBUTTONUP)
            {
                ShowContextMenu(hwnd);
                return IntPtr.Zero;
            }

            if (eventId == WM_LBUTTONDBLCLK)
            {
                App.MainWindowInstance?.Activate();

                return IntPtr.Zero;
            }
        }

        if (msg == WM_COMMAND)
        {
            var commandId = (uint)(wParam.ToInt64() & 0xFFFF);
            if (commandId == ExitCommandId)
            {
                Application.Current.Exit();
                return IntPtr.Zero;
            }
        }

        return _native.DefWindowProc(hwnd, msg, wParam, lParam);
    }

    private void ShowContextMenu(IntPtr hwnd)
    {
        if (!_native.TryGetCursorPos(out var pt))
        {
            return;
        }

        using var menu = _native.CreatePopupMenu();
        menu.AppendItem(ExitCommandId, "Exit");

        _native.SetForegroundWindow(hwnd);
        _native.TrackPopupMenu(menu.Handle, pt.X, pt.Y, hwnd);
    }

    private static string? GetDefaultTrayIconPath()
    {
        return null;
    }

    private delegate IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

    public interface ITrayIconNative
    {
        IntPtr CreateMessageWindow(Delegate wndProc);
        void DestroyMessageWindow(IntPtr hwnd);

        IntPtr LoadIconFromFile(string path);
        IntPtr LoadApplicationIcon();
        void DestroyIcon(IntPtr hIcon);

        bool AddNotifyIcon(IntPtr hwnd, uint iconId, uint callbackMessage, IntPtr hIcon, string tooltip);
        bool RemoveNotifyIcon(IntPtr hwnd, uint iconId);

        IntPtr DefWindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        bool TryGetCursorPos(out POINT pt);
        ITrayPopupMenu CreatePopupMenu();
        void TrackPopupMenu(IntPtr hMenu, int x, int y, IntPtr hwnd);
        void SetForegroundWindow(IntPtr hwnd);
    }

    public interface ITrayPopupMenu : IDisposable
    {
        IntPtr Handle { get; }
        void AppendItem(uint commandId, string text);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    private sealed partial class TrayPopupMenu(IntPtr handle) : ITrayPopupMenu
    {
        public IntPtr Handle { get; } = handle;

        public void AppendItem(uint commandId, string text)
        {
            if (!AppendMenu(Handle, MF_STRING, (IntPtr)commandId, text))
            {
                throw new InvalidOperationException("Failed to append menu item.");
            }
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                DestroyMenu(Handle);
            }
        }
    }

    private sealed class TrayIconNative : ITrayIconNative
    {
        private const string WindowClassName = "ZeeMadArchivist_TrayIconWindow";

        public IntPtr CreateMessageWindow(Delegate wndProc)
        {
            var hInstance = GetModuleHandle(null);

            if (wndProc is not WndProc typedProc)
            {
                throw new ArgumentException("wndProc must be a WndProc delegate.", nameof(wndProc));
            }

            var wc = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
                lpfnWndProc = typedProc,
                hInstance = hInstance,
                lpszClassName = WindowClassName,
            };

            _ = RegisterClassEx(ref wc);

            return CreateWindowEx(
                0,
                WindowClassName,
                string.Empty,
                0,
                0,
                0,
                0,
                0,
                HWND_MESSAGE,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero);
        }

        public void DestroyMessageWindow(IntPtr hwnd)
        {
            DestroyWindow(hwnd);
        }

        public IntPtr LoadIconFromFile(string path)
        {
            return LoadImage(IntPtr.Zero, path, IMAGE_ICON, 0, 0, LR_LOADFROMFILE | LR_DEFAULTSIZE);
        }

        public IntPtr LoadApplicationIcon()
        {
            try
            {
                var processPath = Environment.ProcessPath;
                if (string.IsNullOrWhiteSpace(processPath) || !File.Exists(processPath))
                {
                    return IntPtr.Zero;
                }

                var largeIcons = new IntPtr[1];
                var smallIcons = new IntPtr[1];
                var extracted = ExtractIconEx(processPath, 0, largeIcons, smallIcons, 1);
                if (extracted <= 0)
                {
                    return LoadShellAssociatedIcon(processPath);
                }

                if (smallIcons[0] != IntPtr.Zero)
                {
                    if (largeIcons[0] != IntPtr.Zero)
                    {
                        DestroyIconNative(largeIcons[0]);
                    }

                    return smallIcons[0];
                }

                if (largeIcons[0] != IntPtr.Zero)
                {
                    return largeIcons[0];
                }

                return LoadShellAssociatedIcon(processPath);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        private static IntPtr LoadShellAssociatedIcon(string filePath)
        {
            try
            {
                var result = SHGetFileInfo(
                    filePath,
                    0,
                    out var info,
                    (uint)Marshal.SizeOf<SHFILEINFO>(),
                    SHGFI_ICON | SHGFI_SMALLICON);

                if (result == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }

                return info.hIcon;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        public void DestroyIcon(IntPtr hIcon)
        {
            DestroyIconNative(hIcon);
        }

        public bool AddNotifyIcon(IntPtr hwnd, uint iconId, uint callbackMessage, IntPtr hIcon, string tooltip)
        {
            var data = new NOTIFYICONDATA
            {
                cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATA>(),
                hWnd = hwnd,
                uID = iconId,
                uFlags = NIF_MESSAGE | NIF_TIP | (hIcon != IntPtr.Zero ? NIF_ICON : 0),
                uCallbackMessage = callbackMessage,
                hIcon = hIcon,
                szTip = tooltip ?? string.Empty,
            };

            return Shell_NotifyIcon(NIM_ADD, ref data);
        }

        public bool RemoveNotifyIcon(IntPtr hwnd, uint iconId)
        {
            var data = new NOTIFYICONDATA
            {
                cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATA>(),
                hWnd = hwnd,
                uID = iconId,
            };

            return Shell_NotifyIcon(NIM_DELETE, ref data);
        }

        public IntPtr DefWindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return DefWindowProcW(hwnd, msg, wParam, lParam);
        }

        public bool TryGetCursorPos(out POINT pt)
        {
            return GetCursorPos(out pt);
        }

        public ITrayPopupMenu CreatePopupMenu()
        {
            var hMenu = CreatePopupMenuNative();
            if (hMenu == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create popup menu.");
            }

            return new TrayPopupMenu(hMenu);
        }

        public void TrackPopupMenu(IntPtr hMenu, int x, int y, IntPtr hwnd)
        {
            _ = TrackPopupMenuEx(hMenu, TPM_RIGHTBUTTON, x, y, hwnd, IntPtr.Zero);
        }

        public void SetForegroundWindow(IntPtr hwnd)
        {
            _ = SetForegroundWindowNative(hwnd);
        }
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int ExtractIconEx(string lpszFile, int nIconIndex, [Out] IntPtr[]? phiconLarge, [Out] IntPtr[]? phiconSmall, uint nIcons);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr SHGetFileInfo(
        string pszPath,
        uint dwFileAttributes,
        out SHFILEINFO psfi,
        uint cbFileInfo,
        uint uFlags);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    private const uint SHGFI_ICON = 0x000000100;
    private const uint SHGFI_SMALLICON = 0x000000001;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WNDCLASSEX
    {
        public uint cbSize;
        public uint style;
        public WndProc lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NOTIFYICONDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uID;
        public uint uFlags;
        public uint uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
    }

    private const uint WM_APP = 0x8000;
    private const uint WM_COMMAND = 0x0111;
    private const uint WM_RBUTTONUP = 0x0205;
    private const uint WM_LBUTTONDBLCLK = 0x0203;

    private static readonly IntPtr HWND_MESSAGE = new(-3);

    private const uint NIF_MESSAGE = 0x00000001;
    private const uint NIF_ICON = 0x00000002;
    private const uint NIF_TIP = 0x00000004;

    private const uint NIM_ADD = 0x00000000;
    private const uint NIM_DELETE = 0x00000002;

    private const uint IMAGE_ICON = 1;
    private const uint LR_LOADFROMFILE = 0x00000010;
    private const uint LR_DEFAULTSIZE = 0x00000040;

    private const uint TPM_RIGHTBUTTON = 0x0002;

    private const uint MF_STRING = 0x00000000;

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpwcx);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowEx(
        uint dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr DefWindowProcW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern bool Shell_NotifyIcon(uint dwMessage, [In] ref NOTIFYICONDATA lpData);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadImage(IntPtr hInst, string name, uint type, int cx, int cy, uint fuLoad);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIconNative(IntPtr hIcon);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreatePopupMenu")]
    private static extern IntPtr CreatePopupMenuNative();

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool AppendMenu(IntPtr hMenu, uint uFlags, IntPtr uIDNewItem, string lpNewItem);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyMenu(IntPtr hMenu);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetForegroundWindow")]
    private static extern bool SetForegroundWindowNative(IntPtr hWnd);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);
}
