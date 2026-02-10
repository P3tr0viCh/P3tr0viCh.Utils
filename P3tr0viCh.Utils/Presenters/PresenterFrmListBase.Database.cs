using P3tr0viCh.Utils.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P3tr0viCh.Utils.Presenters
{
    public abstract partial class PresenterFrmListBase<T>
    {
        private readonly WrapperCancellationTokenSource ctsListLoad = new WrapperCancellationTokenSource();

        protected virtual object StatusStartLoad()
        {
            return 0;
        }

        protected virtual object StatusStartSave()
        {
            return 0;
        }

        protected virtual object StatusStartDelete()
        {
            return 0;
        }

        protected virtual void StatusStop(object status)
        {
        }

        protected virtual async Task<IEnumerable<T>> DatabaseListLoadAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult(Enumerable.Empty<T>());
        }

        protected virtual async Task DatabaseListItemsSaveAsync(IEnumerable<T> list)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task DatabaseListItemDeleteAsync(T value)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task DatabaseListItemsDeleteAsync(IEnumerable<T> list)
        {
            await Task.CompletedTask;
        }

        private async Task PerformDatabaseListLoadAsync()
        {
            ctsListLoad.Start();

            var status = StatusStartLoad();

            try
            {
                Application.DoEvents();

                var list = await DatabaseListLoadAsync(ctsListLoad.Token);

                if (ctsListLoad.IsCancellationRequested) return;

                BindingSource.DataSource = list;

                PresenterDataGridView.Sort();

                BindingSource.Position = 0;

                OnListChangedEvent();

                Changed = false;
            }
            catch (TaskCanceledException e)
            {
                DebugWrite.Error(e);
            }
            catch (Exception e)
            {
                OnItemsExceptionLoadEvent(e);
            }
            finally
            {
                ctsListLoad.Finally();

                StatusStop(status);
            }
        }

        private async Task PerformDatabaseListItemsSaveAsync(IEnumerable<T> list)
        {
            var status = StatusStartSave();

            try
            {
                await DatabaseListItemsSaveAsync(list);
            }
            finally
            {
                StatusStop(status);
            }
        }

        private async Task PerformDatabaseListItemsDeleteAsync(IEnumerable<T> list)
        {
            var status = StatusStartDelete();

            try
            {
                await DatabaseListItemsDeleteAsync(list);
            }
            finally
            {
                StatusStop(status);
            }
        }

        internal void OnItemsExceptionLoadEvent(Exception e)
        {
            var eventArgs = new ExceptionEventArgs(e);

            ItemsExceptionLoad?.Invoke(this, eventArgs);
        }

        internal void OnItemsExceptionChangeEvent(Exception e)
        {
            var eventArgs = new ExceptionEventArgs(e);

            ItemsExceptionChange?.Invoke(this, eventArgs);
        }

        internal void OnItemsExceptionDeleteEvent(Exception e)
        {
            var eventArgs = new ExceptionEventArgs(e);

            ItemsExceptionDelete?.Invoke(this, eventArgs);
        }
    }
}