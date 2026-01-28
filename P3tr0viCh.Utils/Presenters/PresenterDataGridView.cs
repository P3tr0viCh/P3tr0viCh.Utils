using P3tr0viCh.Utils.Comparers;
using P3tr0viCh.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace P3tr0viCh.Utils.Presenters
{
    public interface IPresenterDataGridViewCompare<T> where T : IBaseId
    {
        int Compare(T x, T y, string dataPropertyName, ComparerSortOrder sortOrder);
    }

    public abstract class PresenterDataGridView<T> : IPresenterDataGridViewCompare<T> where T : IBaseId
    {
        public DataGridView DataGridView { get; private set; }

        public PresenterDataGridView(DataGridView dataGridView)
        {
            DataGridView = dataGridView;

            DataGridView.CellMouseDown += new DataGridViewCellMouseEventHandler(DataGridView_CellMouseDown);

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
        
        public bool CanSort { get; set; } = true;

        private string sortColumn = string.Empty;

        public string SortColumn
        {
            get => sortColumn;
            set
            {
                if (sortColumn == value)
                {
                    SortOrder = SortOrder == ComparerSortOrder.Ascending ?
                        ComparerSortOrder.Descending :
                        ComparerSortOrder.Ascending;

                    return;
                }

                sortColumn = value;
            }
        }

        public ComparerSortOrder SortOrder { get; set; } = ComparerSortOrder.Ascending;

        public abstract int Compare(T x, T y, string dataPropertyName, ComparerSortOrder sortOrder);

        public void Sort()
        {
            if (!CanSort) return;

            if (DataGridView.IsEmpty()) return;

            if (SortColumn.IsEmpty()) return;

            if (!DataGridView.ColumnExists(SortColumn)) return;

            if (DataGridView.Columns[SortColumn].SortMode == DataGridViewColumnSortMode.NotSortable) return;

            var bindingSource = DataGridView.BindingSource();

            if (bindingSource == null) return;

            var selectedList = SelectedList;

            var list = bindingSource.Cast<T>().ToList();

            list.Sort((T x, T y) =>
            {
                var propertyName = DataGridView.Columns[SortColumn].DataPropertyName;

                return Compare(x, y, propertyName, SortOrder);
            });

            bindingSource.DataSource = list;

            DataGridView.Columns[SortColumn].HeaderCell.SortGlyphDirection =
                SortOrder == ComparerSortOrder.Descending ?
                    System.Windows.Forms.SortOrder.Descending :
                    System.Windows.Forms.SortOrder.Ascending;

            SelectedList = selectedList;
        }

        private void DataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    DataGridView.CurrentCell = DataGridView[e.ColumnIndex, e.RowIndex];
                }
            }
        }

        private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (!CanSort) return;

            SortColumn = DataGridView.Columns[e.ColumnIndex].Name;

            Sort();
        }
    }
}