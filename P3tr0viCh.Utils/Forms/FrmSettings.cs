using System;
using System.Drawing;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public class FrmSettings : Form
    {

        public delegate void BeforeOpenEventHandler(FrmSettings frm);
        public delegate void AfterCloseEventHandler(FrmSettings frm);
        public delegate void LoadFormStateEventHandler(FrmSettings frm);
        public delegate void SaveFormStateEventHandler(FrmSettings frm);

        public delegate bool CheckSettingsEventHandler(FrmSettings frm);

        public class Options
        {
            public BeforeOpenEventHandler BeforeOpen;
            public AfterCloseEventHandler AfterClose;

            public LoadFormStateEventHandler LoadFormState;
            public SaveFormStateEventHandler SaveFormState;

            public CheckSettingsEventHandler CheckSettings;
        }

        private Options options;

        private Panel panelBottom;
        private Button btnOk;
        private Button btnCancel;
        private PropertyGrid propertyGrid;

        private ISettingsBase Settings;

        public FrmSettings()
        {
            InitializeComponent();
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
            propertyGrid.Name = "propertyGrid";
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

        private void BtnOk_Click(object sender, EventArgs e)
        {
            var checkSettings = options.CheckSettings;

            var canClose = checkSettings?.Invoke(this);

            if (canClose != false)
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = Settings;

            propertyGrid.ExpandAllGridItems();
        }

        public static bool Show(IWin32Window owner, ISettingsBase settings, Options options = null)
        {
            using (var frm = new FrmSettings())
            {
                frm.options = options ?? new Options();

                frm.options.BeforeOpen?.Invoke(frm);

                try
                {
                    frm.Settings = settings;

                    frm.Settings.Save();

                    frm.options.LoadFormState?.Invoke(frm);

                    if (frm.ShowDialog(owner) == DialogResult.OK)
                    {
                        frm.options.SaveFormState?.Invoke(frm);

                        frm.Settings.Save();

                        return true;
                    }
                    else
                    {
                        frm.Settings.Load();

                        frm.options.SaveFormState?.Invoke(frm);

                        return false;
                    }
                }
                finally
                {
                    frm.options.AfterClose?.Invoke(frm);
                }
            }
        }
    }
}