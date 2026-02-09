using P3tr0viCh.Utils.EventArguments;

namespace P3tr0viCh.Utils.Delegates
{
    public delegate void ItemChangedEventHandler<T>(object sender, ItemEventArgs<T> e);
}