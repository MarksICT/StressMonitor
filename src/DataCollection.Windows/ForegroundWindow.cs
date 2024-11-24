using ErrorOr;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Threading;

namespace DataCollection.Windows;

public class ForegroundWindow
{
    public static ErrorOr<string> GetWindowTitle()
    {
        PWSTR title;
        unsafe
        {
            var str = stackalloc char[256];
            title = str;
        }
        var foregroundWindowHandle = PInvoke.GetForegroundWindow();
        var result = PInvoke.GetWindowText(foregroundWindowHandle, title, 256);
        if (result <= 0)
        {
            return Error.Failure(description: Win32Helpers.GetErrorMessage(Marshal.GetLastWin32Error()));
        }

        var titleString = title.AsSpan().ToString();
        return titleString;
    }

    public static ErrorOr<string> GetFileName()
    {
        var getProcessIdResult = GetProcessId();
        if (getProcessIdResult.IsError)
        {
            return getProcessIdResult.Errors;
        }

        var hProcess = PInvoke.OpenProcess_SafeHandle(PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_LIMITED_INFORMATION, false,
            getProcessIdResult.Value);
        if (hProcess == null)
        {
            return Error.Failure(description: Win32Helpers.GetErrorMessage(Marshal.GetLastWin32Error()));
        }
        try
        {
            uint capacity = 1024;
            PWSTR exeNameBuffer;
            unsafe
            {
                var str = stackalloc char[(int)capacity];
                exeNameBuffer = str;
            }
            bool success = PInvoke.QueryFullProcessImageName(hProcess, PROCESS_NAME_FORMAT.PROCESS_NAME_WIN32,
                exeNameBuffer, ref capacity);
            if (!success)
            {
                return Error.Failure(description: Win32Helpers.GetErrorMessage(Marshal.GetLastWin32Error()));
            }

            var fullPath = new string(exeNameBuffer.AsSpan()[..(int)capacity]);
            var processName = Path.GetFileName(fullPath);
            return processName;
        }
        finally
        {
            // Close the process handle
            hProcess.Close();
        }
    }

    private static unsafe ErrorOr<uint> GetProcessId()
    {
        var processId = stackalloc uint[1];
        var foregroundWindowHandle = PInvoke.GetForegroundWindow();
        var getProcessIdResult = PInvoke.GetWindowThreadProcessId(foregroundWindowHandle, processId);
        if (getProcessIdResult == 0)
        {
            return Error.Failure(description: Win32Helpers.GetErrorMessage(Marshal.GetLastWin32Error()));
        }

        return *processId;
    }
}