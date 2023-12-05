using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public class SettingsBase<T> where T : new()
    {
        public class FormState
        {
            public Rectangle Bounds { get; set; } = default;
            public bool Maximized { get; set; } = false;
        }

        public class ColumnState
        {
            public int Index { get; set; } = default;
            public int Width { get; set; } = default;
            public bool Visible { get; set; } = true;
            public int DisplayIndex { get; set; } = default;
        }

        private static T defaultInstance = new T();

        public static T Default
        {
            get
            {
                return defaultInstance;
            }
        }

        private static string directory = string.Empty;
        public static string Directory
        {
            get
            {
                return directory;
            }
            set
            {
                directory = value;
                filePath = string.Empty;
            }
        }

        private static string fileName = string.Empty;
        public static string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                filePath = string.Empty;
            }
        }

        private static string filePath = string.Empty;
        public static string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    if (string.IsNullOrEmpty(Directory))
                    {
                        directory = Files.AppDataDirectory();
                    }

                    if (string.IsNullOrEmpty(FileName))
                    {
                        fileName = Files.SettingsFileName();
                    }

                    filePath = Path.Combine(Directory, FileName);
                }

                return filePath;
            }
        }

        public static Exception LastError { get; private set; } = null;

        public static FormState SaveFormState(Form form)
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

                        state.Bounds = form.Bounds;
                    }

                    break;
                case FormBorderStyle.Fixed3D:
                case FormBorderStyle.FixedSingle:
                case FormBorderStyle.FixedDialog:
                case FormBorderStyle.FixedToolWindow:
                    state.Bounds = new Rectangle(form.Left, form.Top, 0, 0);

                    break;
                case FormBorderStyle.SizableToolWindow:
                    state.Bounds = form.Bounds;

                    break;
                default:
                    break;
            }

            return state;
        }

        public static void LoadFormState(Form form, FormState state)
        {
            try
            {
                if (state == null)
                {
                    state = new FormState();
                }

                if (state.Bounds == default)
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
            catch (Exception)
            {
            }
        }

        public static ColumnState[] SaveDataGridColumns(DataGridView dataGridView)
        {
            var columns = new ColumnState[dataGridView.Columns.Count];

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                columns[column.Index] = new ColumnState
                {
                    Index = column.Index,
                    Width = column.Width,
                    Visible = column.Visible,
                    DisplayIndex = column.DisplayIndex
                };
            }

            return columns;
        }

        public static void LoadDataGridColumns(DataGridView dataGridView, ColumnState[] columns)
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
            catch
            {
            }
        }

        public bool Save()
        {
            LastError = null;

            try
            {
                if (!System.IO.Directory.Exists(Directory))
                {
                    System.IO.Directory.CreateDirectory(Directory);
                }

                using (var writer = File.CreateText(FilePath))
                {
                    var content = JsonConvert.SerializeObject(defaultInstance, Formatting.Indented);

                    writer.Write(content);
                }

                return true;
            }
            catch (Exception e)
            {
                LastError = e;

                return false;
            }
        }

        public bool Load()
        {
            LastError = null;

            try
            {
                if (!File.Exists(FilePath)) throw new FileNotFoundException();

                using (var reader = File.OpenText(FilePath))
                {
                    defaultInstance = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                }

                return true;
            }
            catch (Exception e)
            {
                LastError = e;

                return false;
            }
        }
    }
}