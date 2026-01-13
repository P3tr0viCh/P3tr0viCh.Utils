using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using P3tr0viCh.Utils.Extensions;

namespace P3tr0viCh.Utils
{
    public class UserMessages
    {
        public class UserMessage
        {
            public int Id { get; private set; }
            public string Message { get; private set; }
            public IntPtr WParam { get; set; } = IntPtr.Zero;
            public IntPtr LParam { get; set; } = IntPtr.Zero;

            public event ReceivedUserMessageEventHandler ReceivedUserMessage;

            public UserMessage(string message)
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
                return NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, Id, WParam, LParam);
            }

            internal void OnReceivedUserMessage(ReceivedUserMessageEventArgs e)
            {
                ReceivedUserMessage?.Invoke(this, e);
            }
        }

        public class ReceivedUserMessageEventArgs : EventArgs
        {
            private readonly Message message;
            private readonly UserMessage windowMessage;

            public Message Message => message;
            public UserMessage UserMessage => windowMessage;

            public ReceivedUserMessageEventArgs(UserMessage windowMessage, Message message)
            {
                this.message = message;
                this.windowMessage = windowMessage;
            }
        }

        public delegate void ReceivedUserMessageEventHandler(object sender, ReceivedUserMessageEventArgs e);

        public List<UserMessage> Messages { get; set; } = new List<UserMessage>();

        public void WndProc(ref Message m)
        {
            var msgId = m.Msg;

            foreach (var msg in Messages.Where(msg => msg.Id == msgId))
            {
                msg.OnReceivedUserMessage(new ReceivedUserMessageEventArgs(msg, m));
            }
        }
    }
}