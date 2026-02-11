using P3tr0viCh.Utils.EventArguments;
using System.Threading.Tasks;

namespace P3tr0viCh.Utils.Delegates
{
    public delegate Task AsyncItemsDialogEventHandler<T>(object sender, ItemsDialogEventArgs<T> e);
}