using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls;
using System;

namespace CyberFeedForward.TheMadArchivist.Converters;

public sealed partial class IsFolderToSymbolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isFolder)
        {
            return isFolder ? Symbol.Folder : Symbol.Document;
        }

        return Symbol.Document;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }
}
