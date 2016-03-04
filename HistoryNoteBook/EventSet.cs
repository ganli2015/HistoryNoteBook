using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HistoryNoteBook
{
    public class EventSet
    {
        SortedDictionary<int, List<Event>> _events=new SortedDictionary<int, List<Event>>();

        public void Add(Event ev)
        {
            if (!_events.ContainsKey(ev.Year))
            {
                _events[ev.Year] = new List<Event>();
            }

            _events[ev.Year].Add(ev);
        }

        public List<Event> GetEvents(int year)
        {
            return _events[year];
        }
    }
}
