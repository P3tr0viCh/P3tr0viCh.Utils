using P3tr0viCh.Utils.EventArguments;

namespace P3tr0viCh.Utils.Delegates
{
    public delegate void ItemListChangedEventHandler<T>(object sender, ItemListChangedEventArgs<T> e);
}