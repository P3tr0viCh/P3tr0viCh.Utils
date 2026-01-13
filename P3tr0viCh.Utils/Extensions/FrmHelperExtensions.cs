using System;
using System.Windows.Forms;

namespace P3tr0viCh.Utils.Extensions
{
    public static class FrmHelperExtensions
    {
        public static bool IsEmpty(this TextBox textBox) => textBox.GetTrimText().IsEmpty();

        public static bool IsInt(this TextBox textBox) => textBox.Text.IsInt();

        public static bool IsDouble(this TextBox textBox) => textBox.Text.IsDouble();

        public static string GetTrimText(this TextBox textBox) => textBox.Text.Trim();

        public static string GetTrimTextNullable(this TextBox textBox)
        {
            var result = GetTrimText(textBox);

            if (result.IsEmpty())
            {
                return null;
            }

            return result;
        }

        public static double GetDouble(this TextBox textBox)
        {
            if (textBox.IsEmpty()) return 0.0;

            return double.TryParse(textBox.Text, out var result) ? result : 0.0;
        }

        public static double? GetDoubleNullable(this TextBox textBox)
        {
            var result = GetDouble(textBox);

            if (result == 0.0)
            {
                return null;
            }

            return result;
        }

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
            if (comboBox.SelectedItem == null) return default;
            
            if (comboBox.SelectedItem is T selected) return selected;

            throw new InvalidCastException();
        }

        public static bool GetBool(this CheckBox checkBox) => checkBox.Checked;

        public static bool? GetBoolNullable(this CheckBox checkBox)
        {
            if (checkBox.CheckState == CheckState.Indeterminate) return null;

            return checkBox.Checked;
        }

        public static DateTime GetDateTime(this DateTimePicker dateTimePicker)
        {
            return dateTimePicker.Checked ? dateTimePicker.Value : default;
        }

        public static DateTime? GetDateTimeNullable(this DateTimePicker dateTimePicker)
        {
            var result = GetDateTime(dateTimePicker);

            if (result == default)
            {
                return null;
            }

            return result;
        }

        public static void SetText(this TextBox textBox, string value) => textBox.Text = value;

        public static void SetDouble(this TextBox textBox, double? value, string format = null, bool showZero = false, bool showPlus = false)
        {
            string s;

            if (value == null || value == 0)
            {
                s = showZero ? 0d.ToString(format) : string.Empty;
            }
            else
            {
                s = value?.ToString(format);

                if (value > 0 && showPlus)
                {
                    s = "+" + s;
                }
            }

            textBox.SetText(s);
        }

        public static void SetInt(this TextBox textBox, int? value, bool showZero = false, bool showPlus = false)
        {
            string s;

            if (value == null || value == 0)
            {
                s = showZero ? 0.ToString() : string.Empty;
            }
            else
            {
                s = value.ToString();

                if (value > 0 && showPlus)
                {
                    s = "+" + s;
                }
            }

            textBox.SetText(s);
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

        public static void SetEnabledAndVisible(this ToolStripItem Item, bool value)
        {
            Item.Enabled = value;
            Item.Visible = value;
        }

        public static void SetDispayStyle(this ToolStrip toolStrip, ToolStripItemDisplayStyle displayStyle)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                item.DisplayStyle = displayStyle;
            }
        }

        public static void SetShowText(this ToolStrip toolStrip, bool show)
        {
            toolStrip.SetDispayStyle(show ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image);
        }
    }
}