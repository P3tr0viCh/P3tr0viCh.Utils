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
        IPresenterFrmList,
        IPresenterDataGridViewCompare<T> where T : IBaseId, new()
    {
        public IFrmList FrmList { get; private set; }

        public Form Form => FrmList as Form;

        public bool Changed { get; set; } = false;

        public DataGridView DataGridView => FrmList.DataGridView;

        public BindingSource BindingSource { get; private set; } = new BindingSource();

        public PresenterDataGridView<T> PresenterDataGridView { get; private set; }

        public event ItemChangeDialogEventHandler<T> ItemChangeDialog;

        public event ItemListChangeDialogEventHandler<T> ItemListChangeDialog;

        public event ItemListDeleteDialogEventHandler<T> ItemListDeleteDialog;

        public PresenterFrmList(IFrmList frmList)
        {
            FrmList = frmList;

            Form.Load += new EventHandler(FrmList_Load);
            Form.FormClosed += new FormClosedEventHandler(FrmList_FormClosed);

            PresenterDataGridView = new PresenterDataGridViewFrmList<T>(this);

            DataGridView.CellDoubleClick += new DataGridViewCellEventHandler(DataGridView_CellDoubleClick);
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

            DataGridView.DataSource = BindingSource;
        }

        protected abstract string FormTitle { get; }

        protected abstract void LoadFormState();

        protected abstract void SaveFormState();

        protected abstract void UpdateColumns();

        protected virtual void FormOpened()
        {
        }

        private async Task PerformFormLoadAsync()
        {
            FormOpened();

            Form.Text = FormTitle;

            DataGridView.MultiSelect = true;

            SetDataSource();

            LoadFormState();

            UpdateColumns();

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

        public event FrmListChangedEventHandler FrmListChanged;

        private void PerformOnListChanged()
        {
            Changed = true;

            FrmListChanged?.Invoke(this);
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

            DataGridView.SetSelectedRows(value);

            PresenterDataGridView.Sort();

            PerformOnListChanged();
        }

        private void ListItemChange(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                InternalListItemChange(item);
            }

            DataGridView.SetSelectedRows(list);

            PresenterDataGridView.Sort();

            PerformOnListChanged();
        }

        private void ListItemDelete(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                BindingSource.Remove(item);
            }

            PerformOnListChanged();
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

            DataGridView.SetSelectedRows(item);

            await ListItemChangeAsync(item);
        }

        private async Task ListItemChangeSelectedListAsync()
        {
            var list = SelectedList;

            if (!list.Any()) return;

            DataGridView.SetSelectedRows(list);

            await ListItemChangeAsync(list);
        }

        public async Task ListItemChangeSelectedAsync()
        {
            if (!CanChange) return;

            if (Grants.HasFlag(FrmListGrant.MultiChange) && DataGridView.SelectedCount() > 1)
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

                DataGridView.SetSelectedRows(list);

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