namespace P3tr0viCh.Utils.EventArguments
{
    public class ItemDialogEventArgs<T> : OkEventArgs
    {
        public T Value { get; set; } = default;

        public ItemDialogEventArgs()
        {
        }
    }
}