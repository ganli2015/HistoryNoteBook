using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace HistoryNoteBook
{
    public class BuildTagTreeView: BuildTreeView
    {
        List<string> _tags;

        public BuildTagTreeView(List<string> tags)
        {
            _tags = tags;
        }

        override public void Build(EventTree tree, TreeView treeView)
        {
            EventTreeNode root = tree.GetRoot();
            TreeViewItem origin = new TreeViewItem();
            origin.MouseEnter += new MouseEventHandler(Item_MouseEnter);
            origin.Header = TrimString(root.Content);
            origin.IsExpanded = true;
            origin.Tag = root;
            base.BuildTreeItem(root, origin);

            TreeViewItem rootItem = new TreeViewItem();
            rootItem.MouseEnter += new MouseEventHandler(Item_MouseEnter);
            rootItem.Header = TrimString(root.Content);
            rootItem.IsExpanded = true;
            rootItem.Tag = root;
            BuildTagTree(origin, rootItem);

            treeView.Items.Add(rootItem);
        }

        private void BuildTagTree(TreeViewItem origin,TreeViewItem tagTree)
        {
            foreach (TreeViewItem childItem in origin.Items)
            {
                EventTreeNode node = childItem.Tag as EventTreeNode;
                if (!HasIntersection(_tags, node.Tags))
                {
                    BuildTagTree(childItem, tagTree);
                }
                else
                {
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.MouseEnter += new MouseEventHandler(Item_MouseEnter);
                    newItem.Header = TrimString(node.Content);
                    newItem.Tag = node;

                    tagTree.Items.Add(newItem);
                    BuildTagTree(childItem, newItem);
                }
            }
        }

        private bool HasIntersection(List<string> tags1,List<Tag> tags2)
        {
            bool hasIntersecion = false;
            tags1.ForEach(t1 =>
            {
                if (tags2.Find(t2 => t2.Text == t1) != null)
                {
                    hasIntersecion= true;
                }
            });

            return hasIntersecion;
        }

    }
}
