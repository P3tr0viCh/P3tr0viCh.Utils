using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public static class Msg
    {
        public static void Info(string text = "Hello, world!")
        {
            MessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Info(string format, object arg0)
        {
            Info(string.Format(format, arg0));
        }

        public static void Info(string format, object arg0, object arg1)
        {
            Info(string.Format(format, arg0, arg1));
        }

        public static bool Question(string text = "To be or not to be?")
        {
            return MessageBox.Show(text, Application.ProductName,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) ==
                DialogResult.Yes;
        }

        public static bool Question(string format, object arg0)
        {
            return Question(string.Format(format, arg0));
        }

        public static bool Question(string format, object arg0, object arg1)
        {
            return Question(string.Format(format, arg0, arg1));
        }

        public static void Error(string text = "Error!")
        {
            MessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Error(string format, object arg0)
        {
            Error(string.Format(format, arg0));
        }

        public static void Error(string format, object arg0, object arg1)
        {
            Error(string.Format(format, arg0, arg1));
        }
    }
}