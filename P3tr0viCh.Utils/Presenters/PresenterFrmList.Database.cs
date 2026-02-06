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

        protected virtual async Task<IEnumerable<T>> ListLoadAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult(Enumerable.Empty<T>());
        }

        protected virtual async Task ListItemSaveAsync(T value)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task ListItemSaveAsync(IEnumerable<T> list)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task ListItemDeleteAsync(IEnumerable<T> list)
        {
            await Task.CompletedTask;
        }

        private async Task PerformListLoadAsync()
        {
            ctsListLoad.Start();

            var status = StatusStartLoad();

            try
            {
                Application.DoEvents();

                var list = await ListLoadAsync(ctsListLoad.Token);

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
                ListLoadException(e);
            }
            finally
            {
                ctsListLoad.Finally();

                StatusStop(status);
            }
        }

        private async Task PerformListItemSaveAsync(T value)
        {
            var status = StatusStartSave();

            try
            {
                await ListItemSaveAsync(value);
            }
            finally
            {
                StatusStop(status);
            }
        }

        private async Task PerformListItemSaveAsync(IEnumerable<T> list)
        {
            var status = StatusStartSave();

            try
            {
                await ListItemSaveAsync(list);
            }
            finally
            {
                StatusStop(status);
            }
        }

        private async Task PerformListItemDeleteAsync(IEnumerable<T> list)
        {
            var status = StatusStartDelete();

            try
            {
                await ListItemDeleteAsync(list);
            }
            finally
            {
                StatusStop(status);
            }
        }
    }
}