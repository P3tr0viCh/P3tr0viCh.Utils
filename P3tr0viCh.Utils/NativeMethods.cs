using System.Runtime.InteropServices;
using System;

namespace P3tr0viCh.Utils
{
    public static class NativeMethods
    {
        public static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        [DllImport("user32")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool turnOn);
    }
}