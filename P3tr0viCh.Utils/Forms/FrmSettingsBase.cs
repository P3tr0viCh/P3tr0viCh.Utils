using System;
using System.Drawing;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public class FrmSettingsBase : Form
    {
        private Panel panelBottom;

        private Button btnOk;
        private Button btnCancel;
        
        private PropertyGrid propertyGrid;

        public PropertyGrid PropertyGrid => propertyGrid;

        private readonly ISettingsBase settings;
        
        public ISettingsBase Settings => settings;

        private FrmSettingsBase()
        {
            InitializeComponent();
        }

        public FrmSettingsBase(ISettingsBase settings) : this()
        {
            this.settings = settings;
        }

        private void InitializeComponent()
        {
            panelBottom = new Panel();
            btnOk = new Button();
            btnCancel = new Button();
            propertyGrid = new PropertyGrid();

            panelBottom.SuspendLayout();
            SuspendLayout();

            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Size = new Size(488, 48);
            panelBottom.TabIndex = 2;

            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOk.Location = new Point(312, 8);
            btnOk.Margin = new Padding(3, 4, 3, 4);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(80, 32);
            btnOk.TabIndex = 1;
            btnOk.Text = "OK";

            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(400, 8);
            btnCancel.Margin = new Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 32);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Отмена";

            propertyGrid.Dock = DockStyle.Fill;
            propertyGrid.Location = new Point(0, 50);
            propertyGrid.Margin = new Padding(3, 4, 3, 4);
            propertyGrid.Name = "PropertyGrid";
            propertyGrid.Size = new Size(488, 263);
            propertyGrid.TabIndex = 0;
            propertyGrid.ToolbarVisible = false;

            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(384, 361);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Font = new Font("Segoe UI", 10);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(400, 400);
            Name = "FrmSettings";
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            Text = "Настройки";

            panelBottom.Controls.Add(btnOk);
            panelBottom.Controls.Add(btnCancel);

            Controls.Add(propertyGrid);
            Controls.Add(panelBottom);

            panelBottom.ResumeLayout(false);
            panelBottom.PerformLayout();

            ResumeLayout(false);
            PerformLayout();

            Load += Frm_Load;

            btnOk.Click += BtnOk_Click;
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = settings;

            propertyGrid.ExpandAllGridItems();
        }

        protected virtual void CheckSettings()
        {
        }

        protected virtual void SettingsHasError(Exception e)
        {
            Msg.Error(e.Message);
        }

        private bool PerformCheckSettings()
        {
            try
            {
                CheckSettings();

                return true;
            }
            catch (Exception e)
            {
                SettingsHasError(e);

                return false;
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            var canClose = PerformCheckSettings();

            if (canClose)
            {
                DialogResult = DialogResult.OK;
            }
        }

        protected virtual void BeforeOpen()
        {
        }

        protected virtual void AfterClose()
        {
        }

        protected virtual void SaveFormState()
        {
        }

        protected virtual void LoadFormState()
        {
        }

        protected virtual void BeforeSave()
        {
        }

        public new bool ShowDialog(IWin32Window owner)
        {
            BeforeOpen();

            try
            {
                settings.Save();

                LoadFormState();

                if (base.ShowDialog(owner) == DialogResult.OK)
                {
                    SaveFormState();

                    BeforeSave();

                    settings.Save();

                    return true;
                }
                else
                {
                    settings.Load();

                    SaveFormState();

                    return false;
                }
            }
            finally
            {
                AfterClose();
            }
        }
    }
}