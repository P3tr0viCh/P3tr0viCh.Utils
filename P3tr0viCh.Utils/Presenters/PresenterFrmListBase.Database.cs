using P3tr0viCh.Utils.Delegates;
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

        public enum Status
        {
            Load,
            Save,
            Delete,
        }

        public class StatusEventArgs : EventArgs
        {
            public Status Status { get; set; } = default;

            public object Object { get; set; } = null;

            public StatusEventArgs()
            {
            }

            public StatusEventArgs(Status status)
            {
                Status = status;
            }
        }

        public delegate void StatusEventHandler(object sender, StatusEventArgs e);

        public event StatusEventHandler StatusStart;
        public event StatusEventHandler StatusStop;

        internal void OnStatusStart(StatusEventArgs e)
        {
            StatusStart?.Invoke(this, e);
        }

        internal void OnStatusStop(StatusEventArgs e)
        {
            StatusStop?.Invoke(this, e);
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

            var status = new StatusEventArgs(Status.Load);

            OnStatusStart(status);

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

                OnStatusStop(status);
            }
        }

        private async Task PerformDatabaseListItemsSaveAsync(IEnumerable<T> list)
        {
            var status = new StatusEventArgs(Status.Save);

            OnStatusStart(status);

            try
            {
                await DatabaseListItemsSaveAsync(list);
            }
            finally
            {
                OnStatusStop(status);
            }
        }

        private async Task PerformDatabaseListItemsDeleteAsync(IEnumerable<T> list)
        {
            var status = new StatusEventArgs(Status.Delete);

            OnStatusStart(status);

            try
            {
                await DatabaseListItemsDeleteAsync(list);
            }
            finally
            {
                OnStatusStop(status);
            }
        }
    }
}