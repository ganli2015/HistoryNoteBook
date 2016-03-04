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
    /// AllEventDisplayForm.xaml 的交互逻辑
    /// </summary>
    public partial class AllEventDisplayForm : Window
    {
        DataBaseOperator _database;
        List<Event> _all;

        int _eventsPerPage = 25;
        int _curFrom;
        int _curTo;
        int _pageNum;
        int _curPage;
        public AllEventDisplayForm(DataBaseOperator database)
        {
            InitializeComponent();

            this.Title = "所有事件";
            _database = database;
            List<Event> all = database.SearchAllEvents();
            CommonFunction.SortEvents(all);
            _all = all;
            _pageNum = _all.Count / _eventsPerPage + 1;

            _curPage = 1;
            _curFrom = 0;
            _curTo = _eventsPerPage - 1;
            UpdatePageLabel();
            DisplayEvents();   
        }

        private void UpdatePageLabel()
        {
            string text = _curPage.ToString()+"/" + _pageNum.ToString();
            label_Page.Content = text;
        }

        private void DisplayEvents()
        {
            if (_curFrom > _all.Count - 1)
            {
                return;
            }

            int from = _curFrom, to = _curTo;
            if (from < 0)
            {
                from = 0;
            }
            if (to > _all.Count - 1)
            {
                to = _all.Count;
            }

            double width = 0.99 * listbox1.Width;
            for (int i = from; i < to; ++i)
            {
                string str = CreateEventString(_all[i]);
                listbox1.Items.Add(CommonFunction.CreateTextBlockUI(str, width));
            }
        }

        private string CreateEventString(Event ev)
        {
            string time = ev.Time.ToShortDateString();
            return time + "  " + ev.Content;
        }

        private void UpdateEventToDataBase(Event ev)
        {
            DataBaseOperator.GetInstance().UpdateEvent(ev);
        }

        private void listbox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listbox1.SelectedItem == null) return;

            string content = CommonFunction.GetContent(listbox1.SelectedItem);
            List<string> split = CommonFunction.Split(content,' ');
            if (split.Count != 2) return;

            List<Event> events = DataBaseOperator.GetInstance().SeachEvents(split[1]);
            if (events.Count != 1)
            {
                return;
            }

            Event ev = events[0];
            EventEditor input = new EventEditor(ev, new UpdateMainHandler(UpdateEventToDataBase));
            input.Owner = this;
            input.ShowDialog();
        }

        

        private void button_Next_Click(object sender, RoutedEventArgs e)
        {
            _curFrom += _eventsPerPage;
            _curTo += _eventsPerPage;
            listbox1.Items.Clear();
            DisplayEvents();

            _curPage++;
            UpdatePageLabel();
        }

        private void button_Prev_Click(object sender, RoutedEventArgs e)
        {
            _curFrom -= _eventsPerPage;
            _curTo -= _eventsPerPage;
            listbox1.Items.Clear();
            DisplayEvents();

            _curPage--;
            UpdatePageLabel();
        }

        private void textBox_Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            int pageIndex;
            if (!int.TryParse(textBox_Page.Text, out pageIndex))
            {
                return;
            }
            if (pageIndex > _pageNum || pageIndex==_curPage)
            {
                return;
            }

            _curFrom += (pageIndex - _curPage) * _eventsPerPage;
            _curTo += (pageIndex - _curPage) * _eventsPerPage;
            listbox1.Items.Clear();
            DisplayEvents();

            _curPage = pageIndex;
            UpdatePageLabel();

        }
    }
}
