using ErrorOr;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace DataCollection.Windows;

public class ForegroundWindow
{
    internal static ErrorOr<string> GetWindowTitle(HWND window)
    {
        PWSTR title;
        unsafe
        {
            var str = stackalloc char[256];
            title = str;
        }
        var result = PInvoke.GetWindowText(window, title, 256);
        if (result <= 0)
        {
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode == 0)
            {
                return string.Empty;
            }
            
            return Error.Failure(description: Marshal.GetLastPInvokeErrorMessage());
        }
        
        var titleString = title.AsSpan().ToString();
        return titleString;
    }

    internal static ErrorOr<string> GetProcessFileName(HWND window)
    {
        var getProcessIdResult = GetProcessId(window);
        if (getProcessIdResult.IsError)
        {
            return getProcessIdResult.Errors;
        }

        var process = Process.GetProcessById((int) getProcessIdResult.Value);
        var fileName = process.MainModule?.FileVersionInfo.FileName ?? process.ProcessName;
        return fileName;
    }

    internal static ErrorOr<string> GetProcessFriendlyName(HWND window)
    {
        var getProcessIdResult = GetProcessId(window);
        if (getProcessIdResult.IsError)
        {
            return getProcessIdResult.Errors;
        }

        var process = Process.GetProcessById((int)getProcessIdResult.Value);
        var friendlyName = process.MainModule?.FileVersionInfo.FileDescription ??
                           process.MainModule?.FileVersionInfo.ProductName;

        return friendlyName ?? process.ProcessName;
    }

    private static unsafe ErrorOr<uint> GetProcessId(HWND window)
    {
        var processId = stackalloc uint[1];
        var getProcessIdResult = PInvoke.GetWindowThreadProcessId(window, processId);
        if (getProcessIdResult == 0)
        {
            return Error.Failure(description: Marshal.GetLastPInvokeErrorMessage());
        }

        return *processId;
    }
}