using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DataCollection.Windows;

internal class MessageWindow
{
    private readonly WindowMonitor _windowMonitor;
    private readonly WindowsDataCollector _dataCollector;
    private readonly HWND _hwnd;
    private NOTIFYICONDATAW _notifyIconData;
    private readonly string _toolTipText;
    private OptionsWindow? _optionsWindow;
    public NOTIFYICONDATAW NotifyIconData
    {
        get => _notifyIconData;
        set
        {
            _notifyIconData = value;
            SetToolTipText();
        }
    }

    private readonly WNDPROC _originalWndProc;
    private delegate LRESULT WndProcDelegate(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);
    public const uint WmTrayIcon = 0x8000; // Custom message identifier
    public const uint IdTrayAppIcon = 5000;

    public MessageWindow(bool onlyRunInSystemTray)
    {
        _dataCollector = new WindowsDataCollector();
        _windowMonitor = new WindowMonitor(_dataCollector);
        _hwnd = CreateMessageOnlyWindow();
        _toolTipText = AppDomain.CurrentDomain.FriendlyName;
        var windowProcDelegate = onlyRunInSystemTray ? CreateWndProcDelegateOnlySystemTray() : CreateWndProcDelegateMinimize();
        var originalWndProcPtr = PInvoke.SetWindowLongPtr(_hwnd, WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(windowProcDelegate));
        _originalWndProc = (WNDPROC)Marshal.GetDelegateForFunctionPointer(originalWndProcPtr, typeof(WNDPROC));
        NotifyIconData = InitializeNotifyIconData(_hwnd, IdTrayAppIcon, WmTrayIcon);
        if (onlyRunInSystemTray)
        {
            MinimizeToTray();
        }
    }

    public void StartDataCollection()
    {
        _windowMonitor.StartMonitoring();
    }

    private unsafe HWND CreateMessageOnlyWindow()
    {
        const string className = "MessageOnlyWindow";
        var classNamePtr =  stackalloc char[className.Length + 1];
        for (var i = 0; i < className.Length; i++)
        {
            classNamePtr[i] = className[i];
        }
        var hInstance = Marshal.GetHINSTANCE(typeof(App).Module);
        var wc = new WNDCLASSW
        {
            lpfnWndProc = PInvoke.DefWindowProc,
            lpszClassName = classNamePtr,
            hInstance = new HINSTANCE(hInstance)
        };
        
        var registerClassResult = PInvoke.RegisterClass(in wc);
        if (registerClassResult == 0)
        {
            var message = Marshal.GetLastPInvokeErrorMessage();
            PInvoke.MessageBoxEx(_hwnd,
                $"An error occurred while trying to start the Windows Data Collector.\n\nDetails:\n{message}", "Fatal error",
                MESSAGEBOX_STYLE.MB_ICONERROR | MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_TOPMOST, 0);
            Application.Current.Exit();
        }
        var window = PInvoke.CreateWindowEx(0, classNamePtr, new PCWSTR(), 0, 0, 0, 0, 0, HWND.HWND_MESSAGE, HMENU.Null, new HINSTANCE(hInstance));
        if (window == HWND.Null)
        {
            var message = Marshal.GetLastPInvokeErrorMessage();
            PInvoke.MessageBoxEx(_hwnd,
                $"An error occurred while trying to start the Windows Data Collector.\n\nDetails:\n{message}", "Fatal error",
                MESSAGEBOX_STYLE.MB_ICONERROR | MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_TOPMOST, 0);
            Application.Current.Exit();
        }
        return window;
    }

    private void SetToolTipText()
    {
        // Set the tooltip text
        unsafe
        {
            fixed (char* tipPtr = _notifyIconData.szTip.Value)
            {
                _toolTipText.AsSpan().CopyTo(new Span<char>(tipPtr, _notifyIconData.szTip.Length));
            }
        }
    }

    public void ShowContextMenu()
    {
        var menu = PInvoke.CreatePopupMenu_SafeHandle();
        PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_STRING, 1, "Options");
        PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_STRING, 2, "Exit");

        PInvoke.GetCursorPos(out var pt);
        PInvoke.SetForegroundWindow(_hwnd);
        var cmd = PInvoke.TrackPopupMenuEx(menu,
            (uint)(TRACK_POPUP_MENU_FLAGS.TPM_LEFTALIGN | TRACK_POPUP_MENU_FLAGS.TPM_RETURNCMD), pt.X, pt.Y,
            _hwnd, null);

        if (cmd == 1)
        {
            ShowOptionsWindow();
        }
        else if (cmd == 2)
        {
            ExitApplication();
        }
    }

    private void ShowOptionsWindow()
    {
        _optionsWindow = new OptionsWindow();
        _optionsWindow.Activate();
    }

    private static void ExitApplication()
    {
        Application.Current.Exit();
    }

    private WndProcDelegate CreateWndProcDelegateOnlySystemTray()
    {
        return (hWnd, msg, wParam, lParam) =>
        {
            switch (msg)
            {
                case WmTrayIcon when wParam == IdTrayAppIcon:
                    switch ((uint)lParam.Value)
                    {
                        case PInvoke.WM_RBUTTONUP:
                            ShowContextMenu();
                            break;
                        case PInvoke.WM_LBUTTONDBLCLK:
                            ShowOptionsWindow();
                            break;
                    }
                    return new LRESULT(0);
                case PInvoke.WM_DESTROY:
                    Cleanup(hWnd, NotifyIconData, _originalWndProc);
                    break;
            }
            return PInvoke.CallWindowProc(_originalWndProc, hWnd, msg, wParam, lParam);
        };
    }

    private WndProcDelegate CreateWndProcDelegateMinimize()
    {
        return (hWnd, msg, wParam, lParam) =>
        {
            switch (msg)
            {
                case PInvoke.WM_SYSCOMMAND when (wParam.Value & 0xFFF0) == PInvoke.SC_MINIMIZE:
                    MinimizeToTray(true);
                    return new LRESULT(0);
                case WmTrayIcon when lParam.Value == PInvoke.WM_LBUTTONDBLCLK:
                    RestoreFromTray(hWnd, NotifyIconData);
                    return new LRESULT(0);
                case PInvoke.WM_DESTROY:
                    Cleanup(hWnd, NotifyIconData, _originalWndProc);
                    break;
            }

            return PInvoke.CallWindowProc(_originalWndProc, hWnd, msg, wParam, lParam);
        };
    }

    private static NOTIFYICONDATAW InitializeNotifyIconData(IntPtr hWnd, uint idTrayAppIcon, uint wmTrayIcon)
    {
        var notifyIconData = new NOTIFYICONDATAW
        {
            cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONDATAW)),
            hWnd = new HWND(hWnd),
            uID = idTrayAppIcon,
            uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE | NOTIFY_ICON_DATA_FLAGS.NIF_ICON | NOTIFY_ICON_DATA_FLAGS.NIF_TIP,
            uCallbackMessage = wmTrayIcon,
            hIcon = PInvoke.LoadIcon(HINSTANCE.Null, PInvoke.IDI_APPLICATION)
        };

        return notifyIconData;
    }

    private void MinimizeToTray(bool showNotification = false)
    {
        PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, NotifyIconData);
        PInvoke.ShowWindow(_hwnd, SHOW_WINDOW_CMD.SW_HIDE);
        if (!showNotification)
        {
            return;
        }
        var builder = new AppNotificationBuilder()
            .AddText("The app is still running in the system tray.");

        var notificationManager = AppNotificationManager.Default;
        notificationManager.Show(builder.BuildNotification());
    }

    private static void RestoreFromTray(IntPtr hWnd, NOTIFYICONDATAW notifyIconData)
    {
        PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_DELETE, in notifyIconData);
        PInvoke.ShowWindow(new HWND(hWnd), SHOW_WINDOW_CMD.SW_SHOW);
    }

    private static void Cleanup(IntPtr hWnd, NOTIFYICONDATAW notifyIconData, WNDPROC originalWndProc)
    {
        PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_DELETE, in notifyIconData);
        PInvoke.SetWindowLongPtr(new HWND(hWnd), WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(originalWndProc));
    }

    ~MessageWindow()
    {
        _windowMonitor.StopMonitoring();
        Cleanup(_hwnd, NotifyIconData, _originalWndProc);
    }
}