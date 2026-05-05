using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace CyberFeedForward.TheMadArchivist.Converters;

public sealed class FolderIconBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var kind = parameter as string;

        if (value is bool isFolder && isFolder)
        {
            return string.Equals(kind, "Border", StringComparison.OrdinalIgnoreCase)
                ? new SolidColorBrush(Color.FromArgb(0xFF, 0xB8, 0x86, 0x0B))
                : new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xD7, 0x00));
        }

        return new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0x80, 0x80));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }
}
