using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace HistoryNoteBook
{
    public class BuildTreeView
    {
        protected int _notExpandLevel = 6;

        public BuildTreeView()
        {

        }

        virtual public void Build(EventTree tree, TreeView treeView)
        {
            EventTreeNode root = tree.GetRoot();
            TreeViewItem rootItem = new TreeViewItem();
            rootItem.MouseEnter += new MouseEventHandler(Item_MouseEnter);
            rootItem.Header = TrimString(root.Content);
            rootItem.IsExpanded = true;
            rootItem.Tag = root;
            BuildTreeItem(root, rootItem);

            treeView.Items.Add(rootItem);
        }

        protected void BuildTreeItem(EventTreeNode root, TreeViewItem item)
        {
            List<EventTreeNode> children = root.GetChildren();
            children.ForEach(child =>
            {
                TreeViewItem childItem = new TreeViewItem();
                childItem.MouseEnter += new MouseEventHandler(Item_MouseEnter);
                childItem.Header = TrimString(child.Content);
                childItem.Tag = child;

                if (child.GetLevel() < _notExpandLevel)
                {
                    childItem.IsExpanded = true;
                }

                BuildTreeItem(child, childItem);
                item.Items.Add(childItem);
            });
        }

        protected string TrimString(string str)
        {
            if (str.Length > 10)
            {
                str = GetFirstFewString(str, 10) + "...";
            }

            return str;
        }

        protected string GetFirstFewString(string str, int count)
        {
            string res = "";

            if (count > str.Length)
            {
                count = str.Length;
            }
            for (int i = 0; i < count; ++i)
            {
                res += str[i];
            }

            return res;
        }

        protected void Item_MouseEnter(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item == null) return;

            int id = ((EventTreeNode)item.Tag).ID;
            Event ev = DataBaseOperator.GetInstance().SeachEvents(id);
            if (ev == null) return;

            ToolTip tip = new ToolTip();
            tip.Content = ev.Content;
            item.ToolTip = tip;

        }
    }
}
