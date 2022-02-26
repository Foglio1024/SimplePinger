using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace PingApp
{
    public class FocusMonitor
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private static IntPtr FindTeraWindow()
        {
            var error0 = Marshal.GetLastWin32Error();
            var result = FindWindow("LaunchUnrealUWindowsClient", "TERA");
            var error = Marshal.GetLastWin32Error();
            //if (result == IntPtr.Zero && (error != 0))
            //    throw new Win32Exception();
            return result;
        }
        private static IntPtr FindThisWIndow()
        {
            var error0 = Marshal.GetLastWin32Error();
            var result = FindWindow(null, "PingApp");
            var error = Marshal.GetLastWin32Error();
            //if (result == IntPtr.Zero && (error != 0))
            //    throw new Win32Exception();
            return result;
        }

        public static bool IsTeraActive()
        {
            var teraWindow = FindTeraWindow();
            var activeWindow = GetForegroundWindow();
            return (teraWindow != IntPtr.Zero) && (teraWindow == activeWindow);
        }

        public static bool IsThisActive()
        {
            var thisWindow = FindThisWIndow();
            var activeWindow = GetForegroundWindow();
            return (thisWindow != IntPtr.Zero) && (thisWindow == activeWindow);
        }
    }
}
