using P3tr0viCh.Utils.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public static class TextInputBox
    {
        public static bool Show(ref string text, string label = null, string caption = null)
        {
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

                if (caption == null)
                {
                    var assemblyDecorator = new Misc.AssemblyDecorator();

                    caption = assemblyDecorator.Product;
                }

                frm.Text = caption;

                frm.AcceptButton = btnOk;
                frm.CancelButton = btnCancel;

                lblLabel.Parent = frm;
                lblLabel.Font = frm.Font;
                lblLabel.SetBounds(8, 8, 280, 19);
                lblLabel.Text = label;

                textText.Parent = frm;
                textText.Font = frm.Font;
                textText.SetBounds(8, 32, 280, 25);
                textText.Text = text;

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