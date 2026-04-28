using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;

namespace UnitTests;

internal static class WinUiTestHelper
{
    private const int ClassNotRegisteredHResult = unchecked((int)0x80040154);

    public static void Run(Action testAction)
    {
        try
        {
            testAction();
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
