using P3tr0viCh.Utils.EventArguments;

namespace P3tr0viCh.Utils.Delegates
{
    public delegate void ItemsEventHandler<T>(object sender, ItemsEventArgs<T> e);
}