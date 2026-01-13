using P3tr0viCh.Utils.Extensions;
using System.Drawing;
using System.Windows.Forms;

namespace P3tr0viCh.Utils.Forms
{
    public static class TextInputBox
    {
        public class Settings
        {
            public string Caption { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
            public bool UseSystemPasswordChar { get; set; } = false;
            public bool CanEmpty { get; set; } = false;
        };

        public static bool Show(ref string text, string label)
        {
            return Show(ref text, new Settings { Label = label });
        }

        public static bool Show(ref string text, Settings options = null)
        {
            if (options == null)
            {
                options = new Settings();
            }

            using (var frm = new Form())
            using (var lblLabel = new Label())
            using (var textText = new TextBox())
            using (var btnOk = new Button())
            using (var btnCancel = new Button())
            {
                frm.FormBorderStyle = FormBorderStyle.FixedDialog;
                frm.Font = new Font("Segoe UI", 10);
                frm.MaximizeBox = false;
                frm.MinimizeBox = false;
                frm.Size = new Size(312, 152);
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowInTaskbar = false;

                if (options.Caption.IsEmpty())
                {
                    var assemblyDecorator = new AssemblyDecorator();

                    options.Caption = assemblyDecorator.Product;
                }

                frm.Text = options.Caption;

                frm.AcceptButton = btnOk;
                frm.CancelButton = btnCancel;

                lblLabel.Parent = frm;
                lblLabel.Font = frm.Font;
                lblLabel.SetBounds(8, 8, 280, 19);
                lblLabel.Text = options.Label;

                textText.Parent = frm;
                textText.Font = frm.Font;
                textText.SetBounds(8, 32, 280, 25);
                textText.Text = text;
                textText.UseSystemPasswordChar = options.UseSystemPasswordChar;

                btnOk.Parent = frm;
                btnOk.Font = frm.Font;
                btnOk.SetBounds(120, 72, 80, 32);
                btnOk.Text = Properties.Resources.TextInputBoxBtnOk;
                btnOk.Click += (sender, args) => BtnOk_Click(frm, textText, options.CanEmpty);

                btnCancel.Parent = frm;
                btnCancel.Font = frm.Font;
                btnCancel.SetBounds(208, 72, 80, 32);
                btnCancel.Text = Properties.Resources.TextInputBoxBtnCancel;
                btnCancel.DialogResult = DialogResult.Cancel;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    text = textText.Text;

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static void BtnOk_Click(Form frm, TextBox textBox, bool canEmpty)
        {
            if (!canEmpty)
            {
                if (textBox.IsEmpty())
                {
                    Msg.Error(Properties.Resources.TextInputBoxErrorEmptyText);

                    textBox.Focus();

                    return;
                }
            }
            
            frm.DialogResult = DialogResult.OK;
        }
    }
}