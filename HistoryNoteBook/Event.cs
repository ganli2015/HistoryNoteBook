using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HistoryNoteBook
{
    public class Tag
    {
        public string Text { get; set; }

        public Tag()
        {
            Text = "";
        }

        public Tag(string str)
        {
            Text = str;
        }
    }

    public class MyDateTime
    {
        public int Year { set; get; }
        public int Month { set; get; }
        public int Day { set; get; }

        public MyDateTime(int y,int m,int d)
        {
            Year = y;
            Month = m;
            Day = d;
        }

        public string ToShortDateString()
        {
            return Year.ToString() + "/" + Month.ToString() + "/" + Day.ToString();
        }

        public static bool operator<(MyDateTime left,MyDateTime right)
        {
            long num1 = left.ToLong();
            long num2 = right.ToLong();
            if (num1 >= 0 && num2 >= 0)
            {
                return num1 < num2 ? true : false;
            }
            else if (num1 < 0 && num2 > 0)
            {
                return true;
            }
            else if (num1 > 0 && num2 < 0)
            {
                return false;
            }
            else
            {
                if (left.Year < right.Year)
                {
                    return true;
                }
                else if (left.Year > right.Year)
                {
                    return false;
                }
                else
                {
                    if (left.Month < right.Month)
                    {
                        return true;
                    }
                    else if (left.Month > right.Month)
                    {
                        return false;
                    }
                    else
                    {
                        return left.Day < right.Day ? true : false;
                    }
                }
            }
        }

        public static bool operator <=(MyDateTime left, MyDateTime right)
        {
            if (left < right || left == right)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >=(MyDateTime left, MyDateTime right)
        {
            if(left > right || left == right)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(MyDateTime left, MyDateTime right)
        {
            if (left.Year == right.Year && left.Month == right.Month && left.Day == right.Day)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(MyDateTime left, MyDateTime right)
        {
            return !(left == right);
        }

        public static bool operator >(MyDateTime left, MyDateTime right)
        {
            if (left == right || left < right)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private long ToLong()
        {
            return Year * 1000 + Month * 100 + Day;
        }
    }

    public class Event
    {
        public MyDateTime Time { set; get; }
        public int Year { get { return Time.Year; } }
        public int Month { get { return Time.Month; } }
        public int Day { get { return Time.Day; } }
        public string Content { set; get; }
        public int ID { set; get; }
        public List<Tag> Tags { set; get; }

        public Event()
        {
            ID = -1;
            Tags = new List<Tag>();

        }

        public Event(Event ev)
        {
            Time = ev.Time;
            Content = ev.Content;
            ID = ev.ID;
            Tags = ev.Tags;
        }
    }
}
