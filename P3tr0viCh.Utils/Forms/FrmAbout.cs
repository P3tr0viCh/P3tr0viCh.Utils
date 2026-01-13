using P3tr0viCh.Utils.Extensions;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace P3tr0viCh.Utils.Forms
{
    public static class FrmAbout
    {
        public class Options
        {
            public int AppNameFontSize { get; set; } = 26;
            public int AppNameLineBreak { get; set; } = -1;

            public int FormWidth { get; set; } = 440;

            public enum LicenseType
            {
                None,
                Free,
                Commercial,
            }

            public LicenseType License { get; set; } = LicenseType.Free;

            public string Text { get; set; } = string.Empty;

            public string Link { get; set; } = string.Empty;
            public string LinkText { get; set; } = string.Empty;

            public Options()
            {
            }

            public Options(int appNameFontSize)
            {
                AppNameFontSize = appNameFontSize;
            }
        }

        public static void Show(Options options = null)
        {
            if (options == null)
            {
                options = new Options();
            }

            using (var frm = new Form())
            using (var pnlIcon = new Panel())
            using (var pbIcon = new PictureBox())
            using (var pnlAppName = new Panel())
            using (var lblCopyright = new Label())
            using (var lblLink = new LinkLabel())
            using (var ttlink = new ToolTip())
            using (var lblVersion = new Label())
            using (var lblBuildDate = new Label())
            using (var lblText = new Label())
            using (var btnClose = new Button())
            {
                var rnd = new Random();

                string caption;
                string appName;
                string version;
                string copyright;
                string linkLink;
                string linkText;

                DateTime buildDate;

                if (Control.ModifierKeys.HasFlag(Keys.Shift) &&
                    Control.ModifierKeys.HasFlag(Keys.Control) &&
                    Control.ModifierKeys.HasFlag(Keys.Alt))
                {
                    caption = "Автор";
                    appName = "Дураев Константин Петрович";
                    version = "Рандом говорит нам:";
                    copyright = "";
                    linkLink = "https://github.com/P3tr0viCh";
                    linkText = string.Empty;
                    buildDate = new DateTime(1981, 3, 29);
                    options.AppNameFontSize = 16;
                    options.AppNameLineBreak = 0;

                    options.Text = Str.Random(35, rnd);
                    for (int i = 0; i < 9; i++)
                    {
                        options.Text += Str.Eol + Str.Random(35, rnd);
                    }
                }
                else
                {
                    caption = Properties.Resources.FrmAboutCaption;

                    var assemblyDecorator = new AssemblyDecorator();

                    appName = assemblyDecorator.Title;
                    copyright = assemblyDecorator.Copyright;

                    linkLink = options.Link;
                    linkText = options.LinkText;

                    buildDate = new DateTime(2000, 1, 1)
                        .AddDays(assemblyDecorator.Version.Build).AddSeconds(assemblyDecorator.Version.MinorRevision * 2);

                    switch (options.License)
                    {
                        case Options.LicenseType.Free:
                            options.Text = options.Text.JoinExcludeEmpty(Str.Eol, Properties.Resources.FrmAboutEULA_1_1 + Str.Eol + Properties.Resources.FrmAboutEULA_2 + Str.Eol + Properties.Resources.FrmAboutEULA_3 + Str.Eol + Properties.Resources.FrmAboutEULA_4);
                            break;
                        case Options.LicenseType.Commercial:
                            options.Text = options.Text.JoinExcludeEmpty(Str.Eol, Properties.Resources.FrmAboutEULA_1_2 + Str.Eol + Properties.Resources.FrmAboutEULA_2 + Str.Eol + Properties.Resources.FrmAboutEULA_3 + Str.Eol + Properties.Resources.FrmAboutEULA_4);
                            break;
                    }

                    version = string.Format(Properties.Resources.FrmAboutVersion, assemblyDecorator.VersionString());
                }

                if (options.AppNameLineBreak >= 0)
                {
                    var i = appName.IndexOfNth(Str.Space, options.AppNameLineBreak);

                    if (i >= 0)
                    {
                        appName = appName.Substring(0, i) + Str.Eol + appName.Substring(i + 1);
                    }
                }

                if (options.FormWidth < 440)
                {
                    options.FormWidth = 440;
                }

                frm.FormBorderStyle = FormBorderStyle.FixedDialog;
                frm.Font = new Font("Segoe UI", 10);
                frm.MaximizeBox = false;
                frm.MinimizeBox = false;
                frm.Size = new Size(options.FormWidth, 168);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowInTaskbar = false;

                frm.Text = caption;

                pnlIcon.Parent = frm;
                pnlIcon.SetBounds(8, 8, 64, 64);
                pnlIcon.BorderStyle = BorderStyle.Fixed3D;
                pnlIcon.BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));

                pbIcon.Parent = pnlIcon;
                pbIcon.Dock = DockStyle.Fill;
                pbIcon.SizeMode = PictureBoxSizeMode.CenterImage;
                pbIcon.Image = Icon.ExtractAssociatedIcon(Application.ExecutablePath).ToBitmap();

                var stringFormat = new StringFormat()
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };

                var versionFont = new Font(frm.Font.Name, 8, FontStyle.Bold);

                pnlAppName.Parent = frm;
                pnlAppName.Font = new Font(frm.Font.Name, options.AppNameFontSize, FontStyle.Bold | FontStyle.Italic);
                pnlAppName.BorderStyle = BorderStyle.Fixed3D;
                pnlAppName.SetBounds(80, 8, frm.ClientSize.Width - 88, 64);

                var rectangleAppName = pnlAppName.ClientRectangle;
                var rectangleAppName1 = pnlAppName.ClientRectangle;
                rectangleAppName1.Offset(4, 4);
                var rectangleAppName2 = pnlAppName.ClientRectangle;

                var brushAppName = new LinearGradientBrush(rectangleAppName, pnlIcon.BackColor, Color.Black, LinearGradientMode.Horizontal);

                pnlAppName.Paint += (s, e) =>
                {
                    e.Graphics.FillRectangle(brushAppName, rectangleAppName);

                    e.Graphics.DrawString(appName, pnlAppName.Font, Brushes.Black, rectangleAppName1, stringFormat);
                    e.Graphics.DrawString(appName, pnlAppName.Font, Brushes.White, rectangleAppName2, stringFormat);
                };

                var textWidth = frm.ClientSize.Width - 16;
                var linkWidth = linkLink.IsEmpty() ? 0 : 80;

                lblCopyright.Parent = frm;
                lblCopyright.BackColor = Color.White;
                lblCopyright.Font = new Font(frm.Font.Name, 10, FontStyle.Bold);
                lblCopyright.SetBounds(8, 80, textWidth - linkWidth, 32);
                lblCopyright.Text = copyright;

                if (linkWidth != 0)
                {
                    if (linkText.IsEmpty())
                    {
                        if (linkLink.Contains("github.com"))
                        {
                            linkText = "GitHub";
                        }
                        else
                        {
                            linkText = "link";
                        }
                    }

                    lblLink.Parent = frm;
                    lblLink.BackColor = Color.White;
                    lblLink.Font = new Font(frm.Font.Name, 10, FontStyle.Bold);
                    lblLink.SetBounds(lblCopyright.Left + lblCopyright.Width, 80, linkWidth, 32);
                    lblLink.Text = linkText;
                    lblLink.TextAlign = ContentAlignment.TopRight;
                    lblLink.LinkClicked += (s, e) =>
                    {
                        System.Diagnostics.Process.Start(linkLink);
                    };

                    ttlink.SetToolTip(lblLink, linkLink);
                }

                var buildDateWidth = 88;

                lblVersion.Parent = frm;
                lblVersion.BackColor = Color.White;
                lblVersion.Font = new Font(frm.Font.Name, 10);
                lblVersion.SetBounds(8, lblCopyright.Top + lblCopyright.Height, textWidth - buildDateWidth, 32);
                lblVersion.Text = version;

                lblBuildDate.Parent = frm;
                lblBuildDate.BackColor = Color.White;
                lblBuildDate.Font = new Font(frm.Font.Name, 10);
                lblBuildDate.SetBounds(textWidth - buildDateWidth + 8, lblVersion.Top, buildDateWidth, 32);
                lblBuildDate.TextAlign = ContentAlignment.TopRight;
                lblBuildDate.Text = buildDate.ToString("yyyy.MM.dd");

                lblText.Parent = frm;
                lblText.BackColor = Color.White;
                lblText.Font = new Font(frm.Font.Name, 10);
                lblText.SetBounds(8, lblVersion.Top + lblVersion.Height, textWidth, 0);
                lblText.MinimumSize = new Size(textWidth, 0);
                lblText.MaximumSize = new Size(textWidth, 0);
                lblText.Text = options.Text;
                lblText.AutoSize = true;

                frm.Height += lblCopyright.Height + lblVersion.Height + lblText.Height;

                btnClose.Parent = frm;
                btnClose.Text = Properties.Resources.FrmAboutBtnOk;
                btnClose.DialogResult = DialogResult.OK;
                btnClose.SetBounds(frm.ClientSize.Width - 88, frm.ClientSize.Height - 40, 80, 32);
                btnClose.TabIndex = 0;

                frm.AcceptButton = btnClose;
                frm.CancelButton = btnClose;

                frm.ShowDialog();
            }
        }

        public static void Show() => Show(null);

        public static void Show(int appNameFontSize) => Show(new Options(appNameFontSize));
    }
}