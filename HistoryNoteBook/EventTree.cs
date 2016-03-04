using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HistoryNoteBook
{
    public class EventTree
    {
        EventTreeNode _root;
        TreeMode _mode;
        DataBaseOperator _database;

        public enum TreeMode
        {
            Past,
            Future
        }

        public EventTree(Event ev, TreeMode mode)
        {
            if (ev == null)
            {
                throw new NullReferenceException();
            }

            _root = new EventTreeNode(ev);
            _mode = mode;
            _database = DataBaseOperator.GetInstance();
        }

        public void Build()
        {
            _root.SetLevel(0);
            Recursive_Build(_root);
            if (RearrangeRelation())
            {
                _root.ClearChildren();
                Recursive_Build(_root);             
            }
        }

        public EventTreeNode GetRoot()
        {
            return _root;
        }

        private void Recursive_Build(EventTreeNode root)
        {
            List<Event> adjEvents = _database.SearchAdjacentEvents(root);
            List<Event> preferred = FindPreferredEvents(root, adjEvents);
            preferred.ForEach(e =>
            {
                EventTreeNode n = new EventTreeNode(e);
                root.AppendChild(n);
                Recursive_Build(n);
            });
        }

        private List<Event> FindPreferredEvents(Event root,List<Event> eventlist)
        {
            eventlist.Add(root);
            CommonFunction.SortEvents(eventlist);
            int rootIndex=eventlist.IndexOf(root);

            List<Event> res = new List<Event>();
            if (_mode == TreeMode.Past)
            {
                if (rootIndex != 0)//not first
                {
                    res.AddRange(eventlist.GetRange(0, rootIndex));
                }
            }
            else
            {
                if (rootIndex != eventlist.Count - 1)//not last
                {
                    res.AddRange(eventlist.GetRange(rootIndex + 1, eventlist.Count - rootIndex - 1));
                }
            }

            return res;
        }

        /// <summary>
        /// True for rearranged.
        /// If "A->B->C->...->G" and "A->G", then remove "A->G".
        /// </summary>
        /// <returns></returns>
        private bool RearrangeRelation()
        {
            bool rearranged = false;
            Recursive_RearrangeRelation(GetRoot(), ref rearranged);
            

            return rearranged;
        }

        private List<EventTreeNode> GetAllChildren(EventTreeNode node)
        {
            List<EventTreeNode> res = new List<EventTreeNode>();
            Recursive_GetAllChildren(node, res);
            return res;
        }

        private void Recursive_GetAllChildren(EventTreeNode node, List<EventTreeNode> res)
        {
            res.AddRange(node.GetChildren());

            foreach (EventTreeNode n in node.GetChildren())
            {
                Recursive_GetAllChildren(n, res);
            }
        }

        private void Recursive_RearrangeRelation(EventTreeNode node,ref bool rearranged)
        {
            var children=node.GetChildren();
            if(children==null) return;

            foreach (EventTreeNode child in children)//search children
            {
                var grandChildren = GetAllChildren(child);
                if(grandChildren==null) continue;

                foreach (EventTreeNode grandChild in grandChildren)//search grandchildren
                {
                    if (children.Find(c => c.ID == grandChild.ID)!=null)
                    {
                        if (MessageBox.Show("有关系被删除", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            DataBaseOperator.GetInstance().DeleteEdge(grandChild, node);
                            rearranged = true;
                        }                   
                    }
                }

                Recursive_RearrangeRelation(child, ref rearranged);
            }
        }
    }
}
