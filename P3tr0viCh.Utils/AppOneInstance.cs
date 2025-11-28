using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static P3tr0viCh.Utils.WindowMessages;

namespace P3tr0viCh.Utils
{
    public class AppOneInstance
    {
        private readonly Mutex mutex;
        private readonly WindowMessage messageShowApplication;

        private string GetGuid()
        {
            return new AssemblyDecorator().Assembly.GetCustomAttribute<GuidAttribute>().Value;
        }

        public AppOneInstance()
        {
            mutex = new Mutex(true, GetGuid());

            messageShowApplication = new WindowMessage($"{GetGuid()}.WM_SHOWAPPLICATION");
        }

        public bool IsFirstInstance => mutex.WaitOne(TimeSpan.Zero, true);

        public void Release()
        {
            mutex.ReleaseMutex();
        }

        public void ShowExistsInstance()
        {
            messageShowApplication.PostMessage();
        }
  
        public void CheckMessage(Message m, Form form)
        {
            if (m.Msg == messageShowApplication.Id)
            {
                var handle = Application.OpenForms.Count == 1 ? form.Handle : Application.OpenForms[1].Handle;

                NativeMethods.SwitchToThisWindow(handle, true);
            }
        }
    }
}