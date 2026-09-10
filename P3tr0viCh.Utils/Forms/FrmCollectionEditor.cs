using P3tr0viCh.Utils.Extensions;
using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace P3tr0viCh.Utils.Forms
{
    public class FrmCollectionEditor : CollectionEditor
    {
        /*  CollectionEditor Controls
            Type: Name: Text

            System.Windows.Forms.TableLayoutPanel: overArchingTableLayoutPanel: 
            -System.Windows.Forms.Button: downButton: 
            -System.Windows.Forms.TableLayoutPanel: addRemoveTableLayoutPanel: 
            --System.ComponentModel.Design.CollectionEditor+SplitButton: addButton: &Добавить
            --System.Windows.Forms.Button: removeButton: &Удалить
            -System.Windows.Forms.Label: propertiesLabel: &Свойства:
            -System.Windows.Forms.Label: membersLabel: &Члены:
            -System.ComponentModel.Design.CollectionEditor+FilterListBox: listbox: 
            -System.Windows.Forms.Design.VsPropertyGrid: propertyBrowser: PropertyGrid
            --System.Windows.Forms.PropertyGridInternal.DocComment: : Панель описания
            ---System.Windows.Forms.Label: : 
            ---System.Windows.Forms.Label: : 
            --System.Windows.Forms.PropertyGridInternal.HotCommands: : Command Pane
            ---System.Windows.Forms.LinkLabel: : 
            --System.Windows.Forms.PropertyGridInternal.PropertyGridView: : PropertyGridView
            ---System.Windows.Forms.VScrollBar: : 
            ---System.Windows.Forms.PropertyGridInternal.PropertyGridView+GridViewEdit: : 
            --System.Windows.Forms.PropertyGridToolStrip: : PropertyGridToolBar
            -System.Windows.Forms.TableLayoutPanel: okCancelTableLayoutPanel: 
            --System.Windows.Forms.Button: okButton: ОК
            --System.Windows.Forms.Button: cancelButton: Отмена
            -System.Windows.Forms.Button: upButton:
        */

        public FrmCollectionEditor(Type t) : base(t)
        {
        }

        public string Title { get; set; }

        protected override CollectionForm CreateCollectionForm()
        {
            var frm = base.CreateCollectionForm();

            frm.AutoScaleMode = AutoScaleMode.None;
            frm.FormBorderStyle = FormBorderStyle.SizableToolWindow;

            frm.Font = new Font("Segoe UI", 10);

            frm.Width = 600;
            frm.Height = 400;

            frm.HelpButton = false;

            if (!Title.IsEmpty())
            {
                frm.Text = Title;
            }

            var membersLabel = frm.Controls.Find("membersLabel", true).First();
            membersLabel.Visible = false;

            var propertiesLabel = frm.Controls.Find("propertiesLabel", true).First();
            propertiesLabel.Visible = false;

            var overArchingTableLayoutPanel = frm.Controls.Find("overArchingTableLayoutPanel", true).First() as TableLayoutPanel;
            overArchingTableLayoutPanel.SetBounds(8, 8, frm.ClientSize.Width - 16, frm.ClientSize.Height - 16);

            var addRemoveTableLayoutPanel = frm.Controls.Find("addRemoveTableLayoutPanel", true).First() as TableLayoutPanel;
            addRemoveTableLayoutPanel.Margin = new Padding(0);
            addRemoveTableLayoutPanel.Padding = new Padding(0, 4, 0, 0);
            addRemoveTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Absolute;
            addRemoveTableLayoutPanel.ColumnStyles[0].Width = 88;
            addRemoveTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Absolute;
            addRemoveTableLayoutPanel.ColumnStyles[1].Width = 0;
            addRemoveTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Absolute;
            addRemoveTableLayoutPanel.ColumnStyles[2].Width = 88;

            var addButton = frm.Controls.Find("addButton", true).First();
            addButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            addButton.AutoSize = false;
            addButton.Margin = new Padding(0, 0, 4, 0);
            addButton.SetBounds(addButton.Left, 0, 88, 32);

            var removeButton = frm.Controls.Find("removeButton", true).First();
            removeButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            removeButton.AutoSize = false;
            removeButton.Margin = new Padding(4, 0, 0, 0);
            removeButton.SetBounds(removeButton.Left, 0, 88, 32);

            var upButton = frm.Controls.Find("upButton", true).First();
            upButton.AutoSize = false;
            upButton.Margin = new Padding(4, 0, 4, 0);
            upButton.SetBounds(0, 0, 32, 32);

            var downButton = frm.Controls.Find("downButton", true).First();
            downButton.AutoSize = false;
            downButton.Margin = new Padding(4, 0, 4, 0);
            downButton.SetBounds(0, 0, 32, 32);

            var okCancelTableLayoutPanel = frm.Controls.Find("okCancelTableLayoutPanel", true).First() as TableLayoutPanel;
            okCancelTableLayoutPanel.Margin = new Padding(0);
            okCancelTableLayoutPanel.Padding = new Padding(0, 4, 0, 0);

            var okButton = frm.Controls.Find("okButton", true).First();
            okButton.AutoSize = false;
            okButton.Margin = new Padding(0, 0, 4, 0);
            okButton.SetBounds(okButton.Left, 0, 80, 32);

            var cancelButton = frm.Controls.Find("cancelButton", true).First();
            cancelButton.AutoSize = false;
            cancelButton.Margin = new Padding(4, 0, 0, 0);
            cancelButton.SetBounds(okButton.Left, 0, 80, 32);

            var propertyBrowser = frm.Controls.Find("propertyBrowser", true).First() as PropertyGrid;
            propertyBrowser.ToolbarVisible = false;

            frm.Shown += (s, e) =>
            {
                propertyBrowser.ExpandAllGridItems();
            };

            return frm;
        }
    }
}