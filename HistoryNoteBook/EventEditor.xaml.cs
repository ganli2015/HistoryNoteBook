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

namespace HistoryNoteBook
{
    /// <summary>
    /// NewEventInput.xaml 的交互逻辑
    /// </summary>
    /// 
    public delegate void UpdateTagHander(string str);

    public partial class EventEditor : Window
    {
        private event UpdateMainHandler _updateMainHandler;
        private int EventID { set; get; }

        public EventEditor(Event ev, UpdateMainHandler handler)
        {
            InitializeComponent();
            _updateMainHandler += handler;
            if (ev != null)
            {
                Init(ev);
            }
        }

        private void Init(Event ev)
        {
            Title = "编辑事件";
            textBox_Content.Text = ev.Content;
            textBox_year.Text = ev.Year.ToString();
            textBox_month.Text = ev.Month.ToString();
            textBox_day.Text = ev.Day.ToString();
            EventID = ev.ID;

            listBox_Tag.Items.Clear();
            foreach (Tag t in ev.Tags)
            {
                if (t.Text != null)
                    UpdateListTag(t.Text);
            }
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            string content;
            int year, month, day;
            if (CheckValidData(out content, out year, out month, out day))
            {
                Event ev = new Event();
                ev.Content = content;
                MyDateTime time = new MyDateTime(year, month, day);
                ev.Time = time;
                ev.ID = EventID;

                foreach (Border b in listBox_Tag.Items)
                {
                    if (b == null) continue;

                    string tag = CommonFunction.GetContent(b);
                    if (tag != "")
                    {
                        ev.Tags.Add(new Tag(tag));
                    }
                }

                _updateMainHandler(ev);
                Close();
            }
            else
            {
                MessageBox.Show("无效数据！");
            }
        }

        private bool CheckValidData(out string content,out int year,out int month,out int day)
        {
            content="";
            year=0;
            month=0;
            day=0;

            int charNum=textBox_Content.Text.ToCharArray().Length;
            if (charNum > 100)
            {
                return false;
            }
            else
            {
                content=textBox_Content.Text;
            }

            if(!int.TryParse(textBox_year.Text,out year))
            {
                return false;
            }
            if (!int.TryParse(textBox_month.Text, out month))
            {
                return false;
            }
            if (!int.TryParse(textBox_day.Text, out day))
            {
                return false;
            }

            return true;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void textBox_year_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                textBox_month.Focus();
            }
        }

        private void textBox_month_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                textBox_day.Focus();
            }
        }

        private void UpdateListTag(string str)
        {
            listBox_Tag.Items.Add(CommonFunction.CreateTextBlockUI(str, listBox_Tag.Width * 0.9));
        }

        private void button_AddTag_Click(object sender, RoutedEventArgs e)
        {
            TagEditor editor = new TagEditor(textBox_Content.Text,GetTags(listBox_Tag),new UpdateTagHander(UpdateListTag));
            editor.Title = button_AddTag.Content as string;
            editor.Owner = this;
            editor.ShowDialog();
        }

        private List<Tag> GetTags(ListBox list)
        {
            List<Tag> res = new List<Tag>();
            foreach (Border b in list.Items)
            {
                if (b == null) continue;

                string tag = CommonFunction.GetContent(b);
                if (tag != "")
                {
                    res.Add(new Tag(tag));
                }
            }

            return res;
        }
    }
}
