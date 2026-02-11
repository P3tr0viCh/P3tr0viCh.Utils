using P3tr0viCh.Utils.Comparers;
using P3tr0viCh.Utils.Delegates;
using P3tr0viCh.Utils.EventArguments;
using P3tr0viCh.Utils.Extensions;
using P3tr0viCh.Utils.Forms;
using P3tr0viCh.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P3tr0viCh.Utils.Presenters
{
    public abstract partial class PresenterFrmListBase<T> :
        IPresenterFrmListBase,
        IPresenterDataGridViewCompare<T> where T : IBaseId, new()
    {
        public IFrmList FrmList { get; private set; }

        public Form Form => FrmList as Form;

        public bool Changed { get; private set; } = false;

        public BindingSource BindingSource { get; private set; } = new BindingSource();

        public PresenterDataGridView<T> PresenterDataGridView { get; private set; }

        public event FrmListChangedEventHandler ListChanged;

        public event ItemsExceptionLoadEventHandler ItemsExceptionLoad;
        public event ItemsExceptionChangeEventHandler ItemsExceptionChange;
        public event ItemsExceptionDeleteEventHandler ItemsExceptionDelete;

        public event ItemsDialogChangeEventHandler<T> ItemsChangeDialog;
        public event ItemsDialogDeleteEventHandler<T> ItemsDeleteDialog;

        public event ItemsChangedEventHandler<T> ItemsChanged;
        public event ItemsDeletedEventHandler<T> ItemsDeleted;

        public PresenterFrmListBase(IFrmList frmList)
        {
            FrmList = frmList;

            Form.Load += new EventHandler(FrmList_Load);
            Form.FormClosed += new FormClosedEventHandler(FrmList_FormClosed);

            PresenterDataGridView = new PresenterDataGridViewFrmList<T>(this);

            FrmList.DataGridView.CellDoubleClick += new DataGridViewCellEventHandler(DataGridView_CellDoubleClick);
        }

        private async void FrmList_Load(object sender, EventArgs e)
        {
            await PerformFormLoadAsync();
        }

        private void FrmList_FormClosed(object sender, FormClosedEventArgs e)
        {
            PerformFormClosed();
        }

        private FrmListGrant grants = FrmListGrant.Default;
        protected FrmListGrant Grants
        {
            get => grants;
            set
            {
                grants = value;

                PresenterDataGridView.CanSort = CanSort;

                foreach (ToolStripItem item in FrmList.ToolStrip.Items)
                {
                    if (item.Name == "tsbtnClose")
                    {
                        continue;
                    }

                    if (item.Name == "tsbtnAdd")
                    {
                        item.Visible = CanAdd;
                        continue;
                    }

                    if (item.Name == "tsbtnChange")
                    {
                        item.Visible = CanChange;
                        continue;
                    }

                    if (item.Name == "tsbtnDelete")
                    {
                        item.Visible = CanDelete;
                        continue;
                    }

                    if (item.Name == "toolStripSeparator1")
                    {
                        item.Visible = CanAdd || CanChange || CanDelete;
                        continue;
                    }
                }
            }
        }

        private bool CanAdd => grants.HasFlag(FrmListGrant.Add);
        private bool CanChange => grants.HasFlag(FrmListGrant.Change);
        private bool CanDelete => grants.HasFlag(FrmListGrant.Delete);
        private bool CanSort => grants.HasFlag(FrmListGrant.Sort);

        private void SetDataSource()
        {
            BindingSource.DataSource = Enumerable.Empty<T>();

            FrmList.DataGridView.DataSource = BindingSource;
        }

        protected abstract string FormTitle { get; }

        protected abstract void LoadFormState();

        protected abstract void SaveFormState();

        protected abstract void UpdateColumns();

        public virtual void UpdateSettings()
        {
        }

        protected virtual void FormOpened()
        {
        }

        private async Task PerformFormLoadAsync()
        {
            FormOpened();

            Form.Text = FormTitle;

            FrmList.DataGridView.MultiSelect = true;

            SetDataSource();

            LoadFormState();

            UpdateColumns();

            UpdateSettings();

            await UpdateDataAsync();
        }

        public async Task UpdateDataAsync()
        {
            await PerformDatabaseListLoadAsync();
        }

        protected virtual void FormClosed()
        {
        }

        private void PerformFormClosed()
        {
            ctsListLoad.Cancel();

            SaveFormState();

            FormClosed();
        }

        public T Find(T value)
        {
            return BindingSource.Cast<T>().Where(item => item.Id == value.Id).FirstOrDefault();
        }

        public IList<T> List => (IList<T>)BindingSource.List;

        public T Selected
        {
            get => PresenterDataGridView.Selected;
            set => PresenterDataGridView.Selected = Find(value);
        }

        public IEnumerable<T> SelectedList
        {
            get => PresenterDataGridView.SelectedList;
            set => PresenterDataGridView.SelectedList = value;
        }

        private void InternalListItemChange(T value)
        {
            var item = Find(value);

            if (item == null)
            {
                BindingSource.Add(value);
            }
            else
            {
                var index = BindingSource.IndexOf(item);

                BindingSource.List[index] = value;

                BindingSource.ResetItem(index);
            }
        }

        private void InternalListItemChange(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                InternalListItemChange(item);
            }
        }

        private void PerformListItemsChange(IEnumerable<T> list)
        {
            InternalListItemChange(list);

            FrmList.DataGridView.SetSelectedRows(list);

            PresenterDataGridView.Sort();

            var eventArgs = new ItemsEventArgs<T>(list);

            OnItemsChangedEvent(eventArgs);

            OnListChangedEvent();
        }

        private void InternalListItemDelete(T value)
        {
            BindingSource.Remove(value);
        }

        private void InternalListItemDelete(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                InternalListItemDelete(item);
            }
        }

        private void PerformItemsDelete(IEnumerable<T> list)
        {
            InternalListItemDelete(list);

            var eventArgs = new ItemsEventArgs<T>(list);

            OnItemsDeletedEvent(eventArgs);

            OnListChangedEvent();
        }

        public virtual void ListItemsChange(IEnumerable<IBaseId> list)
        {
            InternalListItemChange((IEnumerable<T>)list);

            PresenterDataGridView.Sort();
        }

        public virtual void ListItemsDelete(IEnumerable<IBaseId> list)
        {
            InternalListItemDelete((IEnumerable<T>)list);
        }

        protected virtual T GetNewItem() => new T();

        internal void OnItemsChangeDialogEvent(ItemsDialogEventArgs<T> e)
        {
            ItemsChangeDialog?.Invoke(this, e);
        }

        internal void OnItemsDeleteDialogEvent(ItemsDialogEventArgs<T> e)
        {
            ItemsDeleteDialog?.Invoke(this, e);
        }

        internal void OnListChangedEvent()
        {
            Changed = true;

            ListChanged?.Invoke(this);
        }

        internal void OnItemsChangedEvent(ItemsEventArgs<T> e)
        {
            ItemsChanged?.Invoke(this, e);
        }

        internal void OnItemsDeletedEvent(ItemsEventArgs<T> e)
        {
            ItemsDeleted?.Invoke(this, e);
        }

        private bool ShowItemsChangeDialog(IEnumerable<T> values)
        {
            var eventArgs = new ItemsDialogEventArgs<T>(values);

            OnItemsChangeDialogEvent(eventArgs);

            return eventArgs.Ok;
        }

        private bool ShowItemsDeleteDialog(IEnumerable<T> values)
        {
            var eventArgs = new ItemsDialogEventArgs<T>(values);

            OnItemsDeleteDialogEvent(eventArgs);

            return eventArgs.Ok;
        }

        private async Task ListItemsChangeAsync(IEnumerable<T> list)
        {
            if (!ShowItemsChangeDialog(list)) return;

            await PerformDatabaseListItemsSaveAsync(list);

            PerformListItemsChange(list);
        }

        public async Task AddNewItemAsync()
        {
            if (!CanAdd) return;

            var item = GetNewItem();

            try
            {
                await ListItemsChangeAsync(Enumerable.Empty<T>().Append(item));
            }
            catch (Exception e)
            {
                OnItemsExceptionChangeEvent(e);
            }
        }

        public async Task SelectedChangeAsync()
        {
            if (!CanChange) return;

            try
            {
                IEnumerable<T> selected;

                if (Grants.HasFlag(FrmListGrant.MultiChange) && FrmList.DataGridView.SelectedCount() > 1)
                {
                    selected = SelectedList;
                }
                else
                {
                    selected = Enumerable.Empty<T>().Append(Selected);
                }

                if (selected.IsEmpty()) return;

                FrmList.DataGridView.SetSelectedRows(selected);

                await ListItemsChangeAsync(selected);
            }
            catch (Exception e)
            {
                OnItemsExceptionChangeEvent(e);
            }
        }

        public async Task SelectedDeleteAsync()
        {
            if (!CanDelete) return;

            try
            {
                IEnumerable<T> selected;

                if (Grants.HasFlag(FrmListGrant.MultiDelete) && FrmList.DataGridView.SelectedCount() > 1)
                {
                    selected = SelectedList;
                }
                else
                {
                    selected = Enumerable.Empty<T>().Append(Selected);
                }

                if (selected.IsEmpty()) return;

                FrmList.DataGridView.SetSelectedRows(selected);

                if (!ShowItemsDeleteDialog(selected)) return;

                await PerformDatabaseListItemsDeleteAsync(selected);

                PerformItemsDelete(selected);
            }
            catch (Exception e)
            {
                OnItemsExceptionDeleteEvent(e);
            }
        }

        private async void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            await SelectedChangeAsync();
        }

        public abstract int Compare(T x, T y, string dataPropertyName, ComparerSortOrder sortOrder);
    }
}