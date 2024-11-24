using System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Diagnostics.Debug;

namespace DataCollection.Windows;

public static class Win32Helpers
{
    public static string GetErrorMessage(int errorCode)
    {
        const int bufferSize = 512;
        PWSTR errorMessage;
        unsafe
        {
            var charArray = stackalloc char[bufferSize];
            errorMessage = charArray;
        }

        var result = FormatMessage(bufferSize, (uint)errorCode, errorMessage);

        if (result == 0)
        {
            // Failed to get the error message
            return $"Unknown error (Code {errorCode})";
        }
        return errorMessage.ToString().Trim();
    }

    private static uint FormatMessage(uint bufferSize, uint errorCode, PWSTR errorMessage)
    {
        unsafe
        {
            return PInvoke.FormatMessage(
                FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_IGNORE_INSERTS,
                IntPtr.Zero.ToPointer(),
                errorCode,
                0, // Language ID (0 means the language neutral)
                errorMessage,
                bufferSize,
                null);
        }
    }
}