using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace HistoryNoteBook
{
    public class CommonFunction
    {
        static public Border CreateTextBlockUI(string text,double width)
        {
            Border res = new Border();
            res.CornerRadius = new CornerRadius(5);
            res.BorderThickness = new Thickness(1);
            res.BorderBrush = new System.Windows.Media.SolidColorBrush(Color.FromRgb(0, 0, 255));
            TextBlock b = new TextBlock();
            b.Text = text;
            b.TextWrapping = TextWrapping.Wrap;
            b.Width = width;
            res.Child = b;
           
//             ContextMenu menu = new ContextMenu();
//             MenuItem item = new MenuItem();
//             item.Header = "编辑";

            return res;
        }

        static public string GetContent(object selectedItem)
        {
            Border b = selectedItem as Border;
            if (b == null)
            {
                return "";
            }

            TextBlock t = b.Child as TextBlock;
            if (t == null)
            {
                return "";
            }

            return t.Text;
        }

        /// <summary>
        /// Sort Events by time,past to future.
        /// </summary>
        /// <param name="events"></param>
        static public void SortEvents(List<Event> events)
        {
            events.Sort((left, right) => 
            {
                if (right.Time < left.Time)
                {
                    return 1;
                }
                else if (right.Time == left.Time)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            });

        }

        static public List<string> Split(string str,char tag)
        {
            List<string> res = new List<string>();
            string[] split = str.Split(tag);
            for (int i = 0; i < split.Length; ++i)
            {
                if (split[i] != "")
                {
                    res.Add(split[i]);
                }
            }

            return res;
        }

        /// <summary>
        /// If ev1 connects to ev2 and ev2 connects to ev3 and ev1 connects to ev3,then
        /// delete the edge between ev1 and ev3.
        /// Make sure that the time order:ev1 the earliest and ev3 the latest.
        /// </summary>
        /// <param name="ev1"></param>
        /// <param name="ev2"></param>
        /// <param name="ev3"></param>
        static public void ReBuildRelation(Event ev1, Event ev2, Event ev3)
        {
            if(!(ev1.Time<ev2.Time && ev2.Time<ev3.Time)) return;

            DataBaseOperator database = DataBaseOperator.GetInstance();

            if (database.EdgeExist(ev1, ev2) && database.EdgeExist(ev2, ev3) && database.EdgeExist(ev1, ev3))
            {
                database.DeleteEdge(ev1, ev3);
            }
        }
    }
}
