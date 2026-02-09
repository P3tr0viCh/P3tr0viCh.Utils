using System;

namespace P3tr0viCh.Utils.EventArguments
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public ExceptionEventArgs(Exception e)
        {
            Exception = e;
        }
    }
}