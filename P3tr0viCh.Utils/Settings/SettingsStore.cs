#if DEBUG
#define ENABLE_CHECK_HASH
#endif

using Newtonsoft.Json;
using P3tr0viCh.Utils.Exceptions;
using P3tr0viCh.Utils.Extensions;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace P3tr0viCh.Utils.Settings
{
    public class SettingsStore<T> : ISettingsStore where T : SettingsBase, new()
    {
        private T settings = new T();

        private string directory = DefaultDirectory;

        private string fileName = DefaultFileName;

        private string filePath = string.Empty;

        private string filePathHash = string.Empty;

        public T Settings => settings;

        public object SelectedObject => settings;

        public static string DefaultDirectory => Files.AppDataLocalDirectory();
        public static string DefaultFileName => Files.SettingsFileName();

        public string Directory
        {
            get => directory;
            set
            {
                directory = value.IsEmpty() ? DefaultDirectory : value;

                UpdateFilePath();
            }
        }

        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value.IsEmpty() ? DefaultFileName : value;

                UpdateFilePath();
            }
        }

        public string FilePath => filePath;

        public string FilePathHash => filePathHash;

        public bool UseHash { get; set; } = false;

        public Exception LastError { get; private set; } = null;

        private void UpdateFilePath()
        {
            filePath = Path.Combine(Directory, FileName);

            filePathHash = filePath + "." + Files.ExtConfigHash;
        }

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
            catch (Exception)
            {
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
            catch
            {
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

        protected virtual void Check()
        {
        }


        private string GetHash(string value) => Crypto.HMACSHA256Hash(value, Crypto.SecurityKey);

        private void SaveSettings()
        {
            var content = JsonConvert.SerializeObject(settings, Formatting.Indented);

            File.WriteAllText(FilePath, content);
        }

        private void SaveHash()
        {
            if (UseHash)
            {
                var content = File.ReadAllText(FilePath);

                var hash = GetHash(content);

                File.WriteAllText(FilePathHash, hash);
            }
            else
            {
                File.Delete(FilePathHash);
            }
        }

        public bool Save()
        {
            LastError = null;

            try
            {
                System.IO.Directory.CreateDirectory(Directory);

                SaveSettings();

                SaveHash();

                return true;
            }
            catch (Exception e)
            {
                LastError = e;

                return false;
            }
        }

#if ENABLE_CHECK_HASH
        private void LoadHash()
        {
            if (!UseHash) return;

            if (!File.Exists(FilePath)) return;

            Files.CheckFileExists(FilePathHash);

            if (Files.FileLength(FilePathHash) == 0) throw new FileZeroLengthException();

            var content = File.ReadAllText(FilePath);

            var hash = GetHash(content);

            var hashSaved = File.ReadAllText(FilePathHash);

            if (hashSaved != hash)
            {
                throw new WrongHashException();
            }
        }
#endif

        private void LoadSettings()
        {
            Files.CheckFileExists(FilePath);

            if (Files.FileLength(FilePath) == 0) throw new FileZeroLengthException();

            var content = File.ReadAllText(FilePath);

            settings = JsonConvert.DeserializeObject<T>(content);

            if (settings == null) throw new NullReferenceException();

            Check();
        }

        public bool Load()
        {
            LastError = null;

            try
            {
#if ENABLE_CHECK_HASH
                LoadHash();
#endif

                LoadSettings();

                return true;
            }
            catch (Exception e)
            {
                LastError = e;

                settings = new T();

                return false;
            }
        }
    }
}