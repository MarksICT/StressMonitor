using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using WinRT.Interop;

namespace DataCollection.Windows;

public static class WindowExtensions
{
    private delegate LRESULT WndProcDelegate(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);
    public const uint WmTrayIcon = 0x8000; // Custom message identifier
    public const uint IdTrayAppIcon = 5000;

    private class WindowInfo
    {
        private NOTIFYICONDATAW _notifyIconData;
        private readonly string _toolTipText;
        private OptionsWindow? _optionsWindow;
        public IntPtr HWnd { get; }
        public NOTIFYICONDATAW NotifyIconData 
        { 
            get => _notifyIconData;
            set
            {
                _notifyIconData = value;
                SetToolTipText();
            }
        }
        public WNDPROC OriginalWndProc { get; }
        
        public WindowInfo(Window window, Func<WindowInfo, WndProcDelegate> getWndProcDelegate, string toolTipText)
        {
            _toolTipText = toolTipText;
            var windowProcDelegate = getWndProcDelegate.Invoke(this);
            HWnd = WindowNative.GetWindowHandle(window);
            var originalWndProcPtr = PInvoke.SetWindowLongPtr(new HWND(HWnd), WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(windowProcDelegate));
            OriginalWndProc = (WNDPROC)Marshal.GetDelegateForFunctionPointer(originalWndProcPtr, typeof(WNDPROC));
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
            PInvoke.SetForegroundWindow(new HWND(HWnd));
            var cmd = PInvoke.TrackPopupMenuEx(menu,
                (uint) (TRACK_POPUP_MENU_FLAGS.TPM_LEFTALIGN | TRACK_POPUP_MENU_FLAGS.TPM_RETURNCMD), pt.X, pt.Y,
                new HWND(HWnd), null);

            if (cmd == 1)
            {
                ShowOptionsWindow();
            }
            else if (cmd == 2)
            {
                ExitApplication();
            }
        }

        public void ShowOptionsWindow()
        {
            _optionsWindow = new OptionsWindow();
            _optionsWindow.Activate();
        }

        private static void ExitApplication()
        {
            Application.Current.Exit();
        }
    }

    public static void SetWindowToMinimizeToTray(this Window window)
    {
        var info = new WindowInfo(window, CreateWndProcDelegateMinimize, AppDomain.CurrentDomain.FriendlyName);
        info.NotifyIconData = InitializeNotifyIconData(info.HWnd, IdTrayAppIcon, WmTrayIcon);
        window.Closed += (_, _) => Cleanup(info.HWnd, info.NotifyIconData, info.OriginalWndProc);
    }

    public static void SetWindowToOnlyRunInTray(this Window window)
    {
        var info = new WindowInfo(window, CreateWndProcDelegateOnlySystemTray, AppDomain.CurrentDomain.FriendlyName);
        info.NotifyIconData = InitializeNotifyIconData(info.HWnd, IdTrayAppIcon, WmTrayIcon);
        window.Closed += (_, _) => Cleanup(info.HWnd, info.NotifyIconData, info.OriginalWndProc);
        PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, info.NotifyIconData);
        PInvoke.ShowWindow(new HWND(info.HWnd), SHOW_WINDOW_CMD.SW_HIDE);
    }

    public static void RestoreFromTray(this Window window)
    {
        var handle = WindowNative.GetWindowHandle(window);
        RestoreFromTray(handle, InitializeNotifyIconData(handle, IdTrayAppIcon, WmTrayIcon));
    }

    private static WndProcDelegate CreateWndProcDelegateOnlySystemTray(WindowInfo info)
    {
        return (hWnd, msg, wParam, lParam) =>
        {
            switch (msg)
            {
                case WmTrayIcon when wParam == IdTrayAppIcon:
                    switch ((uint)lParam.Value)
                    {
                        case PInvoke.WM_RBUTTONUP:
                            info.ShowContextMenu();
                            break;
                        case PInvoke.WM_LBUTTONDBLCLK:
                            info.ShowOptionsWindow();
                            break;
                    }
                    return new LRESULT(0);
                case PInvoke.WM_DESTROY:
                    Cleanup(hWnd, info.NotifyIconData, info.OriginalWndProc);
                    break;
            }
            return PInvoke.CallWindowProc(info.OriginalWndProc, hWnd, msg, wParam, lParam);
        };
    }

    private static WndProcDelegate CreateWndProcDelegateMinimize(WindowInfo info)
    {
        return (hWnd, msg, wParam, lParam) =>
        {
            switch (msg)
            {
                case PInvoke.WM_SYSCOMMAND when (wParam.Value & 0xFFF0) == PInvoke.SC_MINIMIZE:
                    MinimizeToTray(hWnd, info.NotifyIconData);
                    return new LRESULT(0);
                case WmTrayIcon when lParam.Value == PInvoke.WM_LBUTTONDBLCLK:
                    RestoreFromTray(hWnd, info.NotifyIconData);
                    return new LRESULT(0);
                case PInvoke.WM_DESTROY:
                    Cleanup(hWnd, info.NotifyIconData, info.OriginalWndProc);
                    break;
            }

            return PInvoke.CallWindowProc(info.OriginalWndProc, hWnd, msg, wParam, lParam);
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

    private static void MinimizeToTray(IntPtr hWnd, NOTIFYICONDATAW notifyIconData)
    {
        PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, in notifyIconData);
        PInvoke.ShowWindow(new HWND(hWnd), SHOW_WINDOW_CMD.SW_HIDE);
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
}