#if DEBUG
#define ENABLE_CHECK_HASH
#endif

using P3tr0viCh.Utils.Extensions;
using P3tr0viCh.Utils.Storage;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace P3tr0viCh.Utils.Settings
{
    public class SettingsStorage<T> : ObjectStorage<T> where T : ObjectPersistenceBase, new()
    {
        public T Settings => Data;

        private string GetFormName(Form form) => form.GetType().Name;

        private string GetDataGridViewName(DataGridView dataGridView) => dataGridView.Name;

        private FormState SaveFormState(Form form)
        {
            var state = new FormState();

            switch (form.FormBorderStyle)
            {
                case FormBorderStyle.None:
                case FormBorderStyle.Sizable:
                    if (form.WindowState == FormWindowState.Maximized)
                    {
                        state.Maximized = true;
                    }
                    else
                    {
                        state.Maximized = false;

                        state.Bounds = form.WindowState == FormWindowState.Minimized ? form.RestoreBounds : form.Bounds;
                    }

                    break;
                case FormBorderStyle.Fixed3D:
                case FormBorderStyle.FixedSingle:
                case FormBorderStyle.FixedDialog:
                case FormBorderStyle.FixedToolWindow:
                    state.Bounds = new Rectangle(form.Left, form.Top, 0, 0);

                    break;
                case FormBorderStyle.SizableToolWindow:
                    state.Bounds = form.WindowState == FormWindowState.Minimized ? form.RestoreBounds : form.Bounds;

                    break;
                default:
                    break;
            }

            return state;
        }

        public void SaveFormState(Form form, string name, FormStates states)
        {
            if (name.IsEmpty()) name = GetFormName(form);

            var state = SaveFormState(form);

            states[name] = state;
        }

        public void SaveFormState(Form form, FormStates states)
        {
            SaveFormState(form, string.Empty, states);
        }

        private bool IsBoundsOnAnyScreen(Rectangle bounds)
        {
            foreach (var screen in Screen.AllScreens)
            {
                if (bounds.IntersectsWith(screen.WorkingArea)) return true;
            }

            return false;
        }

        private void LoadFormState(Form form, FormState state)
        {
            try
            {
                if (state == null)
                {
                    state = new FormState();
                }

                if (state.Bounds == default || !IsBoundsOnAnyScreen(state.Bounds))
                {
                    state.Bounds = new Rectangle(
                        (Screen.FromControl(form).WorkingArea.Width - form.Width) / 2,
                        (Screen.FromControl(form).WorkingArea.Height - form.Height) / 2,
                        form.Width, form.Height);
                }

                form.StartPosition = FormStartPosition.Manual;

                switch (form.FormBorderStyle)
                {
                    case FormBorderStyle.None:
                    case FormBorderStyle.Sizable:
                        form.Bounds = state.Bounds;

                        if (state.Maximized)
                        {
                            form.WindowState = FormWindowState.Maximized;
                        }

                        break;
                    case FormBorderStyle.Fixed3D:
                    case FormBorderStyle.FixedSingle:
                    case FormBorderStyle.FixedDialog:
                    case FormBorderStyle.FixedToolWindow:
                        form.Location = new Point(state.Bounds.Left, state.Bounds.Top);

                        break;
                    case FormBorderStyle.SizableToolWindow:
                        form.Bounds = state.Bounds;

                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);
            }
        }

        public void LoadFormState(Form form, string name, FormStates states)
        {
            if (name.IsEmpty()) name = GetFormName(form);

            if (!states.TryGetValue(name, out FormState state)) state = new FormState();

            LoadFormState(form, state);
        }

        public void LoadFormState(Form form, FormStates states)
        {
            LoadFormState(form, string.Empty, states);
        }

        private ColumnState[] SaveDataGridColumns(DataGridView dataGridView)
        {
            var columns = new ColumnState[dataGridView.Columns.Count];

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                columns[column.Index] = new ColumnState
                {
                    Index = column.Index,
                    Name = column.Name,
                    Width = column.Width,
                    Visible = column.Visible,
                    DisplayIndex = column.DisplayIndex
                };
            }

            return columns;
        }

        public void SaveDataGridColumns(DataGridView dataGridView, string name, ColumnStates states)
        {
            if (name.IsEmpty()) name = GetDataGridViewName(dataGridView);

            var state = SaveDataGridColumns(dataGridView);

            states[name] = state;
        }

        public void SaveDataGridColumns(DataGridView dataGridView, ColumnStates states)
        {
            SaveDataGridColumns(dataGridView, string.Empty, states);
        }

        private void LoadDataGridColumns(DataGridView dataGridView, ColumnState[] columns)
        {
            try
            {
                if (columns == null) return;

                if (columns.Length < dataGridView.Columns.Count) return;

                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    if (columns[column.Index].Width != default)
                    {
                        column.Width = columns[column.Index].Width;
                    }

                    column.Visible = columns[column.Index].Visible;

                    if (columns[column.Index].DisplayIndex != default)
                    {
                        column.DisplayIndex = columns[column.Index].DisplayIndex;
                    }
                }
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);
            }
        }

        public void LoadDataGridColumns(DataGridView dataGridView, string name, ColumnStates columnStates)
        {
            if (name.IsEmpty()) name = GetDataGridViewName(dataGridView);

            if (!columnStates.TryGetValue(name, out ColumnState[] columns)) columns = null;

            LoadDataGridColumns(dataGridView, columns);
        }

        public void LoadDataGridColumns(DataGridView dataGridView, ColumnStates columnStates)
        {
            LoadDataGridColumns(dataGridView, string.Empty, columnStates);
        }
    }
}