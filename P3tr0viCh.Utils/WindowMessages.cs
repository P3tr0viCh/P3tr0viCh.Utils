using System;

namespace P3tr0viCh.Utils
{
    public class WindowMessages
    {
        public class WindowMessage
        {
            public int Id { get; private set; }
            public string Message { get; private set; }

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
        }
    }
}