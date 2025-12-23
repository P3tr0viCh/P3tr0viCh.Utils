using System.Linq;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public abstract class PresenterDataGridView<T>
    {
        public DataGridView DataGridView { get; private set; }

        public PresenterDataGridView(DataGridView dataGridView)
        {
            DataGridView = dataGridView;

            DataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(DataGridView_ColumnHeaderMouseClick);
        }

        private BindingSource BindingSource => DataGridView.DataSource as BindingSource;

        public int Count => BindingSource.Count;
        public int SelectedCount => DataGridView.SelectedCells
                    .Cast<DataGridViewCell>().Select(cell => cell.OwningRow).Distinct().Count();

        public T Selected
        {
            get => (T)BindingSource.Current;
            set => BindingSource.Position = BindingSource.IndexOf(value);
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

        protected abstract int Compare(T x, T y, string dataPropertyName);

        public void Sort()
        {
            if (Count == 0) return;

            if (SortColumn.IsEmpty()) return;

            if (!DataGridView.ColumnExists(SortColumn)) return;

            var selected = Selected;

            var list = BindingSource.Cast<T>().ToList();

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

            BindingSource.DataSource = list;

            DataGridView.Columns[SortColumn].HeaderCell.SortGlyphDirection = SortOrderDescending ? SortOrder.Descending : SortOrder.Ascending;

            Selected = selected;
        }

        private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            SortColumn = DataGridView.Columns[e.ColumnIndex].Name;

            Sort();
        }
    }
}