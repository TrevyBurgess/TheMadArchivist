using Microsoft.UI.Xaml.Controls;
using System;

namespace CyberFeedForward.TheMadArchivist.Utilities;

public static class FrameNavigationExtensions
{
    public static bool NavigateIfNotCurrent(this Frame frame, Type sourcePageType)
    {
        if (frame is null)
        {
            throw new ArgumentNullException(nameof(frame));
        }

        if (sourcePageType is null)
        {
            throw new ArgumentNullException(nameof(sourcePageType));
        }

        if (frame.CurrentSourcePageType == sourcePageType)
        {
            return false;
        }

        return frame.Navigate(sourcePageType);
    }
}
