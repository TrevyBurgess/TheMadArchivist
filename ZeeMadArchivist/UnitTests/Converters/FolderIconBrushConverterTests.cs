using CyberFeedForward.TheMadArchivist.Converters;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;

namespace UnitTests.Converters;

[TestClass]
public sealed class FolderIconBrushConverterTests
{
    private const int ClassNotRegisteredHResult = unchecked((int)0x80040154);

    [TestMethod]
    public void Convert_IsFolder_Fill_ReturnsBrush()
    {
        try
        {
            var converter = new FolderIconBrushConverter();

            var result = converter.Convert(true, typeof(Brush), "Fill", "en-US");

            Assert.IsInstanceOfType<SolidColorBrush>(result);
        }
        catch (COMException ex) when (ex.HResult == ClassNotRegisteredHResult)
        {
            Assert.Inconclusive("WinUI runtime is not available in the current test environment.");
        }
        catch (TypeInitializationException ex) when (ex.InnerException is COMException { HResult: ClassNotRegisteredHResult })
        {
            Assert.Inconclusive("WinUI runtime is not available in the current test environment.");
        }
    }

    [TestMethod]
    public void Convert_IsFolder_Border_ReturnsBrush()
    {
        try
        {
            var converter = new FolderIconBrushConverter();

            var result = converter.Convert(true, typeof(Brush), "Border", "en-US");

            Assert.IsInstanceOfType<SolidColorBrush>(result);
        }
        catch (COMException ex) when (ex.HResult == ClassNotRegisteredHResult)
        {
            Assert.Inconclusive("WinUI runtime is not available in the current test environment.");
        }
        catch (TypeInitializationException ex) when (ex.InnerException is COMException { HResult: ClassNotRegisteredHResult })
        {
            Assert.Inconclusive("WinUI runtime is not available in the current test environment.");
        }
    }

    [TestMethod]
    public void Convert_NotFolder_ReturnsBrush()
    {
        try
        {
            var converter = new FolderIconBrushConverter();

            var result = converter.Convert(false, typeof(Brush), "Fill", "en-US");

            Assert.IsInstanceOfType<SolidColorBrush>(result);
        }
        catch (COMException ex) when (ex.HResult == ClassNotRegisteredHResult)
        {
            Assert.Inconclusive("WinUI runtime is not available in the current test environment.");
        }
        catch (TypeInitializationException ex) when (ex.InnerException is COMException { HResult: ClassNotRegisteredHResult })
        {
            Assert.Inconclusive("WinUI runtime is not available in the current test environment.");
        }
    }
}
