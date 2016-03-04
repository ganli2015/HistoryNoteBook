using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Windows.Automation.Peers;

namespace HistoryNoteBook
{
    /// <summary>
    /// EventGraphForm.xaml 的交互逻辑
    /// </summary>
    public partial class EventGraphForm : Window
    {
        public EventGraphForm(Event ev)
        {
            InitializeComponent();
            
            EventTree pastTree = new EventTree(ev, EventTree.TreeMode.Past);
            pastTree.Build();
            BuildTreeView buildTreeView=new BuildTreeView();
            buildTreeView.Build(pastTree, treeView_Past);

            EventTree futureTree = new EventTree(ev, EventTree.TreeMode.Future);
            futureTree.Build();
            buildTreeView.Build(futureTree, treeView_Future);
        }

        public EventGraphForm(Event ev, List<string> tags)
        {
            InitializeComponent();

            EventTree pastTree = new EventTree(ev, EventTree.TreeMode.Past);
            pastTree.Build();
            BuildTreeView buildTreeView = new BuildTagTreeView(tags);
            buildTreeView.Build(pastTree, treeView_Past);

            EventTree futureTree = new EventTree(ev, EventTree.TreeMode.Future);
            futureTree.Build();
            buildTreeView.Build(futureTree, treeView_Future);
        }

        private void treeView_Past_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = e.NewValue as TreeViewItem;
            if (item == null) return;

            Event child, parent;//parent is the future and child is the past.
            GetEventPair(item, out child, out parent);
            if (child == null || parent == null)
            {
                return;
            }

            string info = "";
            AppendSelectedInfo(child, ref info);
            AppendSelectedInfo(parent, ref info);
            AppendRelationContent(child, parent, ref info);

            textBlock_Past.Text = info;
        }

        private void treeView_Future_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = e.NewValue as TreeViewItem;
            if (item == null) return;

            Event child, parent;//parent is the past and child is the future.
            GetEventPair(item, out child, out parent);
            if (child == null || parent == null)
            {
                return;
            }

            string info = "";
            AppendSelectedInfo(parent, ref info);
            AppendSelectedInfo(child, ref info);
            AppendRelationContent(child, parent, ref info);

            textBlock_Future.Text = info;
        }

        private void GetEventPair(TreeViewItem item, out Event child, out Event parent)
        {
            child = null;
            parent = null;

            int id = ((EventTreeNode)item.Tag).ID;
            if (id == -1) return;
            child = DataBaseOperator.GetInstance().SeachEvents(id);

            TreeViewItem parentItem = item.Parent as TreeViewItem;
            if (parentItem == null) return;
            int idParent = ((EventTreeNode)parentItem.Tag).ID;
            if (idParent == -1) return;
            parent= DataBaseOperator.GetInstance().SeachEvents(idParent);
        }

        private void AppendSelectedInfo(Event myEvent, ref string info)
        {
            info += myEvent.Time.ToShortDateString();
            info += "\r\n";
            info += myEvent.Content;
            info += "\r\n";
        }

        private void AppendRelationContent(Event child, Event parent, ref string info)
        {
            string content = "";
            string content1 = DataBaseOperator.GetInstance().SearchEdgeInfo(child,parent);
            string content2 = DataBaseOperator.GetInstance().SearchEdgeInfo(parent, child);
            if (content1 != "")
            {
                content = content1;
            }
            else
            {
                content = content2;
            }

            info += "关系：";
            info += "\r\n";
            info += content;
        }

        private void scrollViewer2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            scrollViewer2.ScrollToVerticalOffset(scrollViewer2.VerticalOffset - 5);
        }


    }
}
