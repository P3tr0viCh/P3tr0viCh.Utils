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
    public abstract partial class PresenterFrmList<T> :
        IFrmUpdateData,
        IPresenterFrmList,
        IPresenterDataGridViewCompare<T> where T : IBaseId, new()
    {
        public IFrmList FrmList { get; private set; }

        public Form Form => FrmList as Form;

        public bool Changed { get; private set; } = false;

        public BindingSource BindingSource { get; private set; } = new BindingSource();

        public PresenterDataGridView<T> PresenterDataGridView { get; private set; }

        public event FrmListChangedEventHandler FrmListChanged;

        public event ItemChangeDialogEventHandler<T> ItemChangeDialog;

        public event ItemListChangeDialogEventHandler<T> ItemListChangeDialog;

        public event ItemListDeleteDialogEventHandler<T> ItemListDeleteDialog;

        public event ItemChangedEventHandler<T> ItemChanged;

        public event ItemListChangedEventHandler<T> ItemListChanged;

        public event ItemListDeletedEventHandler<T> ItemListDeleted;

        public PresenterFrmList(IFrmList frmList)
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
            await PerformListLoadAsync();
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

        private void ListItemChange(T value)
        {
            InternalListItemChange(value);

            FrmList.DataGridView.SetSelectedRows(value);

            PresenterDataGridView.Sort();

            var itemChangedEventArgs = new ItemChangedEventArgs<T>()
            {
                Value = value
            };

            OnItemChangedEvent(itemChangedEventArgs);

            OnFrmListChangedEvent();
        }

        private void ListItemChange(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                InternalListItemChange(item);
            }

            FrmList.DataGridView.SetSelectedRows(list);

            PresenterDataGridView.Sort();

            var itemListChangedEventArgs = new ItemListChangedEventArgs<T>()
            {
                Values = list
            };

            OnItemListChangedEvent(itemListChangedEventArgs);

            OnFrmListChangedEvent();
        }

        private void InternalListItemDelete(IBaseId value)
        {
            BindingSource.Remove(value);
        }

        private void ListItemDelete(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                InternalListItemDelete(item);
            }

            var itemListDeletedEventArgs = new ItemListDeletedEventArgs<T>()
            {
                Values = list
            };

            OnItemListDeletedEvent(itemListDeletedEventArgs);

            OnFrmListChangedEvent();
        }

        protected virtual T GetNewItem() => new T();

        internal void OnItemChangeDialogEvent(ItemDialogEventArgs<T> e)
        {
            ItemChangeDialog?.Invoke(this, e);
        }

        internal void OnItemListChangeDialogEvent(ItemListDialogEventArgs<T> e)
        {
            ItemListChangeDialog?.Invoke(this, e);
        }

        internal void OnItemListDeleteDialogEvent(ItemListDialogEventArgs<T> e)
        {
            ItemListDeleteDialog?.Invoke(this, e);
        }

        internal void OnFrmListChangedEvent()
        {
            Changed = true;

            FrmListChanged?.Invoke(this);
        }

        internal void OnItemChangedEvent(ItemChangedEventArgs<T> e)
        {
            ItemChanged?.Invoke(this, e);
        }

        internal void OnItemListChangedEvent(ItemListChangedEventArgs<T> e)
        {
            ItemListChanged?.Invoke(this, e);
        }

        internal void OnItemListDeletedEvent(ItemListDeletedEventArgs<T> e)
        {
            ItemListDeleted?.Invoke(this, e);
        }

        private bool ShowItemChangeDialog(T value)
        {
            var itemDialogEventArgs = new ItemDialogEventArgs<T>()
            {
                Value = value
            };

            OnItemChangeDialogEvent(itemDialogEventArgs);

            return itemDialogEventArgs.Ok;
        }

        private bool ShowItemListChangeDialog(IEnumerable<T> values)
        {
            var itemListDialogEventArgs = new ItemListDialogEventArgs<T>()
            {
                Values = values
            };

            OnItemListChangeDialogEvent(itemListDialogEventArgs);

            return itemListDialogEventArgs.Ok;
        }

        private bool ShowItemDeleteDialog(IEnumerable<T> values)
        {
            var itemListDialogEventArgs = new ItemListDialogEventArgs<T>()
            {
                Values = values
            };

            OnItemListDeleteDialogEvent(itemListDialogEventArgs);

            return itemListDialogEventArgs.Ok;
        }

        private async Task ListItemChangeAsync(T value)
        {
            try
            {
                if (!ShowItemChangeDialog(value)) return;

                await PerformListItemSaveAsync(value);

                ListItemChange(value);
            }
            catch (Exception e)
            {
                ListItemChangeException(e);
            }
        }

        private async Task ListItemChangeAsync(IEnumerable<T> list)
        {
            try
            {
                if (!ShowItemListChangeDialog(list)) return;

                await PerformListItemSaveAsync(list);

                ListItemChange(list);
            }
            catch (Exception e)
            {
                ListItemChangeException(e);
            }
        }

        public async Task ListItemAddNewAsync()
        {
            if (!CanAdd) return;

            var item = GetNewItem();

            await ListItemChangeAsync(item);
        }

        private async Task ListItemChangeSelectedItemAsync()
        {
            var item = Selected;

            if (item == null) return;

            FrmList.DataGridView.SetSelectedRows(item);

            await ListItemChangeAsync(item);
        }

        private async Task ListItemChangeSelectedListAsync()
        {
            var list = SelectedList;

            if (!list.Any()) return;

            FrmList.DataGridView.SetSelectedRows(list);

            await ListItemChangeAsync(list);
        }

        public async Task ListItemChangeSelectedAsync()
        {
            if (!CanChange) return;

            if (Grants.HasFlag(FrmListGrant.MultiChange) && FrmList.DataGridView.SelectedCount() > 1)
            {
                await ListItemChangeSelectedListAsync();
            }
            else
            {
                await ListItemChangeSelectedItemAsync();
            }
        }

        public async Task ListItemDeleteSelectedAsync()
        {
            if (!CanDelete) return;

            try
            {
                var list = SelectedList;

                if (!list.Any()) return;

                FrmList.DataGridView.SetSelectedRows(list);

                if (!ShowItemDeleteDialog(list)) return;

                await PerformListItemDeleteAsync(list);

                ListItemDelete(list);
            }
            catch (Exception e)
            {
                ListItemDeleteException(e);
            }
        }

        protected virtual void ListLoadException(Exception e) { }

        protected virtual void ListItemChangeException(Exception e) { }

        protected virtual void ListItemDeleteException(Exception e) { }

        private async void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            await ListItemChangeSelectedAsync();
        }

        public abstract int Compare(T x, T y, string dataPropertyName, ComparerSortOrder sortOrder);
    }
}