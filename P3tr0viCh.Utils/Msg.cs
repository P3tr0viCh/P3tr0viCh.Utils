using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public static class Msg
    {
        public static void Info(string text = "Hello, world!")
        {
            MessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool Question(string text = "To be or not to be?")
        {
            return MessageBox.Show(text, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes;
        }

        public static void Error(string text = "Error!")
        {
            MessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}