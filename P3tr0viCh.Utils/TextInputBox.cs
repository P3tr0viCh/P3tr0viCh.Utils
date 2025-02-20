using P3tr0viCh.Utils.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public static class TextInputBox
    {
        public class Options
        {
            public string Caption = string.Empty;
            public string Label = string.Empty;
            public bool UseSystemPasswordChar = false;
        };

        public static bool Show(ref string text, string label)
        {
            return Show(ref text, new Options { Label = label });
        }

        public static bool Show(ref string text, Options options = null)
        {
            if (options == null)
            {
                options = new Options();
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
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowInTaskbar = false;

                if (options.Caption.IsEmpty())
                {
                    var assemblyDecorator = new Misc.AssemblyDecorator();

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
                btnOk.SetBounds(104, 72, 88, 32);
                btnOk.Text = Resources.TextInputBoxBtnOk;
                btnOk.DialogResult = DialogResult.OK;

                btnCancel.Parent = frm;
                btnCancel.Font = frm.Font;
                btnCancel.SetBounds(200, 72, 88, 32);
                btnCancel.Text = Resources.TextInputBoxBtnCancel;
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
    }
}