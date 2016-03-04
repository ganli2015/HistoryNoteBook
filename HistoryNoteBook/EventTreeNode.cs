using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HistoryNoteBook
{
    public class EventTreeNode:Event
    {
        List<EventTreeNode> _children;
        int _level=0;

        public EventTreeNode(Event ev):base(ev)
        {
            _children = new List<EventTreeNode>();
        }

        public void AppendChild(EventTreeNode ev)
        {
            ev.SetLevel(_level + 1);
            _children.Add(ev);
        }

        public void SetLevel(int i)
        {
            _level = i;
        }

        public int GetLevel()
        {
            return _level;
        }

        public List<EventTreeNode> GetChildren()
        {
            return _children;
        }

        public void ClearChildren()
        {
            _children.Clear();
        }
    }
}
