using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tools
{
    public static class FolderTools
    {
        public static bool UpdateFolderIcon(string iconFilePath, string folderPath)
        {
            if (string.IsNullOrWhiteSpace(iconFilePath) || string.IsNullOrWhiteSpace(folderPath))
            {
                return false;
            }

            if (!File.Exists(iconFilePath) || !Directory.Exists(folderPath))
            {
                return false;
            }

            try
            {
                var desktopIniPath = Path.Combine(folderPath, "desktop.ini");

                var desktopIniContents = new StringBuilder();
                desktopIniContents.AppendLine("[.ShellClassInfo]");
                desktopIniContents.AppendLine($"IconResource={iconFilePath},0");

                File.WriteAllText(desktopIniPath, desktopIniContents.ToString(), Encoding.Unicode);

                var iniAttributes = File.GetAttributes(desktopIniPath);
                File.SetAttributes(desktopIniPath, iniAttributes | FileAttributes.Hidden | FileAttributes.System);

                var folderAttributes = File.GetAttributes(folderPath);
                File.SetAttributes(folderPath, folderAttributes | FileAttributes.ReadOnly);

                SHChangeNotify(SHCNE_UPDATEITEM, SHCNF_PATHW, folderPath, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int MapDrive(string folderPath, char driveLetter, string name)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                return Win32ErrorInvalidParameter;
            }

            var normalizedLetter = char.ToUpperInvariant(driveLetter);
            if (normalizedLetter is < 'A' or > 'Z')
            {
                return Win32ErrorInvalidParameter;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return Win32ErrorInvalidParameter;
            }

            var localName = normalizedLetter + ":";

            var nr = new NetResource
            {
                dwType = ResourceTypeDisk,
                lpLocalName = localName,
                lpRemoteName = folderPath,
                lpProvider = null,
                lpComment = name,
            };

            return WNetAddConnection2(ref nr, null, null, ConnectUpdateProfile);
        }

        public static bool UnmapDrive(char driveLetter)
        {
            var normalizedLetter = char.ToUpperInvariant(driveLetter);
            if (normalizedLetter is < 'A' or > 'Z')
            {
                return false;
            }

            var localName = normalizedLetter + ":";
            var result = WNetCancelConnection2(localName, CancelUpdateProfile, true);

            return result == 0;
        }

        private const int Win32ErrorInvalidParameter = 87;
        private const int ResourceTypeDisk = 0x00000001;
        private const int ConnectUpdateProfile = 0x00000001;
        private const int CancelUpdateProfile = 0x00000001;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NetResource
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            public string? lpLocalName;
            public string? lpRemoteName;
            public string? lpComment;
            public string? lpProvider;
        }

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetAddConnection2(ref NetResource lpNetResource, string? lpPassword, string? lpUsername, int dwFlags);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

        private const uint SHCNE_UPDATEITEM = 0x00002000;
        private const uint SHCNF_PATHW = 0x0005;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, string? dwItem1, string? dwItem2);
    }
}
