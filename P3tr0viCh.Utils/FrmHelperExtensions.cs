using System;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public static class FrmHelperExtensions
    {
        public static bool IsEmpty(this TextBox textBox) => textBox.Text.IsEmpty();

        public static bool IsInt(this TextBox textBox) => textBox.Text.IsInt();

        public static string GetTrimText(this TextBox textBox) => textBox.Text.TrimText();

        public static int GetInt(this TextBox textBox)
        {
            if (textBox.IsEmpty()) return 0;

            return int.TryParse(textBox.Text, out var result) ? result : 0;
        }

        public static int? GetIntNullable(this TextBox textBox)
        {
            var result = GetInt(textBox);

            if (result == 0)
            {
                return null;
            }

            return result;
        }

        public static string GetTrimText(this ComboBox comboBox) => comboBox.Text.TrimText();

        public static T GetSelectedItem<T>(this ComboBox comboBox)
        {
            return comboBox.SelectedItem != null ? (T)comboBox.SelectedItem : default;
        }

        public static bool GetBool(this CheckBox checkBox) => checkBox.Checked;

        public static DateTime? GetDateTime(this DateTimePicker dateTimePicker)
        {
            if (dateTimePicker.Checked)
            {
                return dateTimePicker.Value;
            }
            else
            {
                return null;
            }
        }

        public static void SetText(this TextBox textBox, string value) => textBox.Text = value;

        public static void SetInt(this TextBox textBox, int? value, bool showZero = false, bool showPlus = false)
        {
            if (value == 0)
            {
                textBox.Text = showZero ? "0" : string.Empty;
            }
            else
            {
                textBox.Text = value.ToString();

                if (value > 0 && showPlus)
                {
                    textBox.Text = "+" + textBox.Text;
                }
            }
        }
        
        public static void SetInt(this TextBox textBox, long? value, bool showZero = false, bool showPlus = false)
        { 
            textBox.SetInt((int?)value, showZero, showPlus);
        }

        public static void SetText(this ComboBox comboBox, string value) => comboBox.Text = value;

        public static void SetBool(this CheckBox checkBox, bool value) => checkBox.Checked = value;

        public static void SetDateTime(this DateTimePicker dateTimePicker, DateTime? dateTime, DateTime defaultValue)
        {
            if (dateTime == default || dateTime == null || dateTime < dateTimePicker.MinDate)
            {
                dateTimePicker.Value = defaultValue;
                dateTimePicker.Checked = false;
            }
            else
            {
                dateTimePicker.Value = (DateTime)dateTime;
                dateTimePicker.Checked = true;
            }
        }

        public static void SetDateTime(this DateTimePicker dateTimePicker, DateTime? dateTime)
        {
            dateTimePicker.SetDateTime(dateTime, DateTime.Now);
        }

        public static void SetDate(this DateTimePicker dateTimePicker, DateTime? dateTime)
        {
            dateTimePicker.SetDateTime(dateTime, DateTime.Today);
        }

        public static void SetEnabledAndVisible(this ToolStripMenuItem menuItem, bool value)
        {
            menuItem.Enabled = value;
            menuItem.Visible = value;
        }
    }
}