using System;

namespace P3tr0viCh.Utils.EventArguments
{
    public class OkEventArgs : EventArgs
    {
        public bool Ok { get; set; } = false;

        public OkEventArgs()
        {
        }
    }
}