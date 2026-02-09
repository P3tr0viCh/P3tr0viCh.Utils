using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P3tr0viCh.Utils.Presenters
{
    public abstract partial class PresenterFrmList<T>
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

        protected virtual async Task DatabaseListItemSaveAsync(T value)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task DatabaseListItemSaveAsync(IEnumerable<T> list)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task DatabaseListItemDeleteAsync(T value)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task DatabaseListItemDeleteAsync(IEnumerable<T> list)
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

                OnFrmListChangedEvent();

                Changed = false;
            }
            catch (TaskCanceledException e)
            {
                DebugWrite.Error(e);
            }
            catch (Exception e)
            {
                DatabaseListLoadException(e);
            }
            finally
            {
                ctsListLoad.Finally();

                StatusStop(status);
            }
        }

        private async Task PerformDatabaseListItemSaveAsync(T value)
        {
            var status = StatusStartSave();

            try
            {
                await DatabaseListItemSaveAsync(value);
            }
            finally
            {
                StatusStop(status);
            }
        }

        private async Task PerformDatabaseListItemSaveAsync(IEnumerable<T> list)
        {
            var status = StatusStartSave();

            try
            {
                await DatabaseListItemSaveAsync(list);
            }
            finally
            {
                StatusStop(status);
            }
        }

        private async Task PerformDatabaseListItemDeleteAsync(T value)
        {
            var status = StatusStartDelete();

            try
            {
                await DatabaseListItemDeleteAsync(value);
            }
            finally
            {
                StatusStop(status);
            }
        }

        private async Task PerformDatabaseListItemDeleteAsync(IEnumerable<T> list)
        {
            var status = StatusStartDelete();

            try
            {
                await DatabaseListItemDeleteAsync(list);
            }
            finally
            {
                StatusStop(status);
            }
        }

        protected virtual void DatabaseListLoadException(Exception e) { }

        protected virtual void DatabaseListItemChangeException(Exception e) { }

        protected virtual void DatabaseListItemDeleteException(Exception e) { }
    }
}