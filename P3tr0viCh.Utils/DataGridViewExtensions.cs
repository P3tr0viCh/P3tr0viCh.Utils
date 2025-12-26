using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public static class DataGridViewExtensions
    {
        private static BindingSource GetBindingSource(DataGridView dataGridView)
        {
            return dataGridView.DataSource is BindingSource binding ? binding : null;
        }

        private static int GetCount(DataGridView dataGridView)
        {
            var binding = GetBindingSource(dataGridView);

            return binding != null ? binding.Count : dataGridView.Rows.Count;
        }

        public static BindingSource BindingSource(this DataGridView dataGridView) => GetBindingSource(dataGridView);

        public static int Count(this DataGridView dataGridView) => GetCount(dataGridView);

        public static bool IsEmpty(this DataGridView dataGridView) => Count(dataGridView) == 0;

        public static IEnumerable<T> GetSelectedList<T>(this DataGridView dataGridView) where T : IBaseId
        {
            if (dataGridView.IsEmpty()) return Enumerable.Empty<T>();

            if (dataGridView.SelectedCells.Count == 0) return Enumerable.Empty<T>();

            var selectedRows = dataGridView.SelectedCells
                .Cast<DataGridViewCell>()
                .Select(cell => cell.OwningRow).Distinct();

            if (selectedRows.Count() == 0) return Enumerable.Empty<T>();

            return selectedRows.Select(item => item.Value<T>());
        }

        public static void SetSelectedList<T>(this DataGridView dataGridView, IEnumerable<T> values) where T : IBaseId
        {
            if (dataGridView.IsEmpty()) return;

            dataGridView.SetSelectedRows(values);
        }

        private static int GetSelectedCount(this DataGridView dataGridView)
        {
            return dataGridView.SelectedCells.Cast<DataGridViewCell>()
                .Select(cell => cell.OwningRow).Distinct().Count();
        }

        public static int SelectedCount(this DataGridView dataGridView) => GetSelectedCount(dataGridView);

        public static bool ColumnExists(this DataGridView dataGridView, string columnName)
        {
            return dataGridView.Columns[columnName] != null;
        }

        public static T GetSelected<T>(this DataGridView dataGridView) where T : IBaseId
        {
            return dataGridView.BindingSource()?.Current is T current ? current : default;
        }

        public static int GetPosition<T>(this DataGridView dataGridView, T value) where T : IBaseId
        {
            var binding = dataGridView.BindingSource();

            if (binding == null) return -1;

            return binding.IndexOf(value);
        }

        public static void SetPosition(this DataGridView dataGridView, int position)
        {
            if (position == -1) return;

            var binding = dataGridView.BindingSource();

            if (binding == null) return;

            binding.Position = position;
        }

        public static void SetSelected<T>(this DataGridView dataGridView, T value) where T : IBaseId
        {
            var index = dataGridView.GetPosition(value);

            dataGridView.SetPosition(index);
        }

        public static DataGridViewRow Find<T>(this DataGridView dataGridView, T value) where T : IBaseId
        {
            if (value == null) return null;

            if (dataGridView.IsEmpty()) return null;

            foreach (var row in from DataGridViewRow row in dataGridView.Rows
                                where row.Value<T>().Id == value.Id
                                select row)
            {
                return row;
            }

            return null;
        }

        public static IEnumerable<DataGridViewRow> Find<T>(this DataGridView dataGridView, IEnumerable<T> values) where T : IBaseId
        {
            if (dataGridView.IsEmpty()) return Enumerable.Empty<DataGridViewRow>();

            if (values?.Any() != true) return Enumerable.Empty<DataGridViewRow>();

            var result = Enumerable.Empty<DataGridViewRow>();

            foreach (var row in from value in values
                                from row in
                                    from DataGridViewRow row in dataGridView.Rows
                                    where row.Value<T>().Id == value.Id
                                    select row
                                select row)
            {
                result = result.Append(row);
            }

            return result;
        }

        public static void SetSelectedRows<T>(this DataGridView dataGridView, IEnumerable<T> values) where T : IBaseId
        {
            dataGridView.ClearSelection();

            var rows = dataGridView.Find(values);

            rows.FirstOrDefault()?.SelectAndScroll();

            foreach (var row in rows)
            {
                row.Selected = true;
            }
        }

        public static void SetSelectedRows<T>(this DataGridView dataGridView, T value) where T : IBaseId
        {
            dataGridView.ClearSelection();

            dataGridView.Find(value)?.SelectAndScroll();
        }
    }

    public static class DataGridViewRowExtensions
    {
        public static T Value<T>(this DataGridViewRow row)
        {
            return (T)row.DataBoundItem;
        }

        public static void SelectAndScroll(this DataGridViewRow row)
        {
            row.Selected = true;

            if (!row.Displayed)
            {
                row.DataGridView.FirstDisplayedScrollingRowIndex = row.Index;
            }

            if (row.DataGridView.DataSource is BindingSource binding)
            {
                binding.Position = row.Index;
            }
        }
    }
}