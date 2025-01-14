using DataCollection.Common;
using System;
using System.Collections.Generic;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Accessibility;

namespace DataCollection.Windows;

public class WindowMonitor(IDataCollector dataCollector)
{
    private HWINEVENTHOOK _hWinEventHook;
    private readonly IDataCollector _dataCollector = dataCollector;
    private string _foregroundWindowTitle = string.Empty;
    private string _foregroundProcessFileName = string.Empty;
    private string _foregroundProcessFriendlyName = string.Empty;
    private DateTimeOffset _currentWindowOpenTime;
    public List<string> Errors { get; } = [];

    public void StartMonitoring()
    {
        _hWinEventHook = PInvoke.SetWinEventHook(
            PInvoke.EVENT_SYSTEM_FOREGROUND,
            PInvoke.EVENT_SYSTEM_FOREGROUND,
            HMODULE.Null,
            WinEventProc,
            0,
            0,
            PInvoke.WINEVENT_OUTOFCONTEXT);
    }

    public void StopMonitoring()
    {
        PInvoke.UnhookWinEvent(_hWinEventHook);
    }

    private void WinEventProc(
        HWINEVENTHOOK hWinEventHook,
        uint eventType,
        HWND hwnd,
        int idObject,
        int idChild,
        uint dwEventThread,
        uint dwmsEventTime)
    {
        try
        {
            var getWindowTitleResult = ForegroundWindow.GetWindowTitle(hwnd);
            var getProcessFilenameResult = ForegroundWindow.GetProcessFileName(hwnd);
            var getProcessFriendlyNameResult = ForegroundWindow.GetProcessFriendlyName(hwnd);
            if (getWindowTitleResult.IsError && getProcessFilenameResult.IsError)
            {
                _foregroundWindowTitle = string.Empty;
                _foregroundProcessFileName = string.Empty;
                _foregroundProcessFriendlyName = string.Empty;
                _currentWindowOpenTime = DateTimeOffset.UtcNow;
                Errors.Add(getWindowTitleResult.FirstError.Description);
                return;
            }
            
            if (getProcessFilenameResult.IsError)
            {
                _foregroundWindowTitle = string.Empty;
                _foregroundProcessFileName = string.Empty;
                _foregroundProcessFriendlyName = string.Empty;
                _currentWindowOpenTime = DateTimeOffset.UtcNow;
                Errors.Add(getProcessFilenameResult.FirstError.Description);
                return;
            }

            if ((getWindowTitleResult.IsError || getWindowTitleResult.Value == string.Empty) &&
                getProcessFilenameResult.Value.Contains("explorer.exe", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (getWindowTitleResult.Value.Equals("Task Switching", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (!string.IsNullOrEmpty(_foregroundProcessFileName))
            {
                var data = new WindowsData(_foregroundWindowTitle, _foregroundProcessFileName,
                    _foregroundProcessFriendlyName, _currentWindowOpenTime, DateTimeOffset.Now);
                _dataCollector.AddData(data);
            }
            _foregroundWindowTitle = getWindowTitleResult.Value;
            _foregroundProcessFileName = getProcessFilenameResult.Value;
            _foregroundProcessFriendlyName = getProcessFriendlyNameResult.Match(value => value, _ => "UNKNOWN");
            _currentWindowOpenTime = DateTimeOffset.UtcNow;
        }
        catch (Exception)
        {
            // Ignore
        }
    }
}