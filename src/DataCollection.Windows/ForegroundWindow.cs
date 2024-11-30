using ErrorOr;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Threading;

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
            return Error.Failure(description: Marshal.GetLastPInvokeErrorMessage());
        }

        var titleString = title.AsSpan().ToString();
        return titleString;
    }

    internal static ErrorOr<string> GetFileName(HWND window)
    {
        var getProcessIdResult = GetProcessId(window);
        if (getProcessIdResult.IsError)
        {
            return getProcessIdResult.Errors;
        }

        var hProcess = PInvoke.OpenProcess_SafeHandle(PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_LIMITED_INFORMATION, false,
            getProcessIdResult.Value);
        if (hProcess == null)
        {
            return Error.Failure(description: Marshal.GetLastPInvokeErrorMessage());
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
                return Error.Failure(description: Marshal.GetLastPInvokeErrorMessage());
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