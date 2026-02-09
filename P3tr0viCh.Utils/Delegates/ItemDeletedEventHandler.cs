using P3tr0viCh.Utils.EventArguments;

namespace P3tr0viCh.Utils.Delegates
{
    public delegate void ItemDeletedEventHandler<T>(object sender, ItemEventArgs<T> e);
}