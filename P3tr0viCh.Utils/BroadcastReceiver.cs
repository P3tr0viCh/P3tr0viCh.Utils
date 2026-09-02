using System;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public class BroadcastReceiver : NativeWindow, IDisposable
    {
        private bool _disposed;

        public BroadcastReceiver()
        {
            var cp = new CreateParams
            {
                Caption = "BroadcastReceiver",
                ClassName = null,
                Style = NativeMethods.WS_POPUP,
                ExStyle = NativeMethods.WS_EX_NOACTIVATE,
                Parent = IntPtr.Zero,
                Width = 0,
                Height = 0
            };

            CreateHandle(cp);
        }

        ~BroadcastReceiver() => Dispose();

        public void Dispose()
        {
            if (_disposed) return;

            DestroyHandle();

            _disposed = true;
        }

        public delegate void ReceivedMessageEventHandler(object sender, ref Message m);

        public event ReceivedMessageEventHandler ReceivedMessage;

        internal void OnReceivedMessage(ref Message m)
        {
            ReceivedMessage?.Invoke(this, ref m);
        }

        protected override void WndProc(ref Message m)
        {
            OnReceivedMessage(ref m);

            base.WndProc(ref m);
        }
    }
}