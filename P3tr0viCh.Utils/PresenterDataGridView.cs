using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public interface IPresenterDataGridViewCompare<T> where T : IBaseId
    {
        int Compare(T x, T y, string dataPropertyName);
    }

    public abstract class PresenterDataGridView<T> : IPresenterDataGridViewCompare<T> where T : IBaseId
    {
        public DataGridView DataGridView { get; private set; }

        public PresenterDataGridView(DataGridView dataGridView)
        {
            DataGridView = dataGridView;

            DataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(DataGridView_ColumnHeaderMouseClick);
        }

        public T Selected
        {
            get => DataGridView.GetSelected<T>();
            set => DataGridView.SetSelected(value);
        }

        public IEnumerable<T> SelectedList
        {
            get => DataGridView.GetSelectedList<T>();
            set => DataGridView.SetSelectedList(value);
        }

        private string sortColumn = string.Empty;

        public string SortColumn
        {
            get => sortColumn;
            set
            {
                if (sortColumn == value)
                {
                    SortOrderDescending = !SortOrderDescending;

                    return;
                }

                sortColumn = value;
            }
        }

        public bool SortOrderDescending { get; set; } = false;

        public abstract int Compare(T x, T y, string dataPropertyName);

        public void Sort()
        {
            if (DataGridView.IsEmpty()) return;

            if (SortColumn.IsEmpty()) return;

            if (!DataGridView.ColumnExists(SortColumn)) return;

            var bindingSource = DataGridView.BindingSource();

            if (bindingSource == null) return;

            var selectedList = SelectedList;

            var list = bindingSource.Cast<T>().ToList();

            list.Sort((T x, T y) =>
            {
                var propertyName = DataGridView.Columns[SortColumn].DataPropertyName;

                var compare = Compare(x, y, propertyName);

                if (SortOrderDescending)
                {
                    compare = -compare;
                }

                return compare;
            });

            bindingSource.DataSource = list;

            DataGridView.Columns[SortColumn].HeaderCell.SortGlyphDirection = SortOrderDescending ? SortOrder.Descending : SortOrder.Ascending;

            SelectedList = selectedList;
        }

        private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            SortColumn = DataGridView.Columns[e.ColumnIndex].Name;

            Sort();
        }
    }
}