using System.Collections.Generic;

namespace P3tr0viCh.Utils.EventArguments
{
    public class ItemListDialogEventArgs<T> : OkEventArgs
    {
        public IEnumerable<T> Values { get; set; } = null;

        public ItemListDialogEventArgs()
        {
        }
    }
}