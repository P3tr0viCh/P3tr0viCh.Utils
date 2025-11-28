using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace P3tr0viCh.Utils
{
    public class WindowMessages
    {
        public class ReceivedWindowMessageEventArgs : EventArgs
        {
            private readonly WindowMessage windowMessage;

            public WindowMessage WindowMessage => windowMessage;

            public ReceivedWindowMessageEventArgs(WindowMessage windowMessage)
            {
                this.windowMessage = windowMessage;
            }
        }

        public delegate void ReceivedWindowMessageEventHandler(object sender, ReceivedWindowMessageEventArgs e);

        public class WindowMessage
        {
            public int Id { get; private set; }
            public string Message { get; private set; }

            public event ReceivedWindowMessageEventHandler ReceivedWindowMessage;

            public WindowMessage(string message)
            {
                if (message.IsEmpty())
                {
                    throw new ArgumentNullException("message");
                }

                Message = message;

                Id = NativeMethods.RegisterWindowMessage(message);
            }

            public bool PostMessage()
            {
                return NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, Id, IntPtr.Zero, IntPtr.Zero);
            }

            internal void OnReceivedWindowMessage(ReceivedWindowMessageEventArgs e)
            {
                ReceivedWindowMessage?.Invoke(this, e);
            }
        }

        public List<WindowMessage> Messages { get; set; } = new List<WindowMessage>();

        public void WndProc(ref Message m)
        {
            var msgId = m.Msg;

            foreach (var msg in Messages.Where(msg => msg.Id == msgId))
            {
                msg.OnReceivedWindowMessage(new ReceivedWindowMessageEventArgs(msg));
            }
        }
    }
}