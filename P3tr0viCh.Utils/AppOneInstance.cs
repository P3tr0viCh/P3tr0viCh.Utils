using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public static class AppOneInstance
    {
        private static string GetGuid()
        {
            return new Misc.AssemblyDecorator().Assembly.GetCustomAttribute<GuidAttribute>().Value;
        }

        private static readonly Mutex mutex = new Mutex(true, GetGuid());

        public static bool IsFirstInstance()
        {
            return mutex.WaitOne(TimeSpan.Zero, true);
        }

        public static void Exit() {
            mutex.ReleaseMutex();
        }

        public static readonly int WM_SHOWAPPLICATION = NativeMethods.RegisterWindowMessage($"{GetGuid()}.WM_SHOWAPPLICATION");

        public static void ShowExistsInstance() {
            NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, WM_SHOWAPPLICATION, IntPtr.Zero, IntPtr.Zero);
        }

        public static void CheckAndShowApplication(Message m, Form form)
        {
            if (m.Msg == WM_SHOWAPPLICATION)
            {
                var handle = Application.OpenForms.Count == 1 ? form.Handle : Application.OpenForms[1].Handle;

                NativeMethods.SwitchToThisWindow(handle, true);
            }
        }
    }
}