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
    /// SearchEventForm.xaml 的交互逻辑
    /// </summary>
    public partial class BuildRelationForm : Window
    {
        public DataBaseOperator databaseOperator { set; get; }
        public Event firstEvent { set; get; }
        public event MakeEdgeHandler _makeEdgeHandler;

        public BuildRelationForm()
        {
            InitializeComponent();
        }

        private void button_Search_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
            List<Event> results = databaseOperator.SearchEvents_ContainStr(textBox_SearchContent.Text);
            results.ForEach(ev =>
            {
                listBox1.Items.Add(CommonFunction.CreateTextBlockUI(ev.Content, listBox1.Width * 0.9));
            });
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string content = CommonFunction.GetContent(listBox1.SelectedItem);
                List<Event> events = databaseOperator.SeachEvents(content);
                if (events.Count == 1)
                {
                    if (events[0].ID!=firstEvent.ID)
                        _makeEdgeHandler(firstEvent, events[0], textBox_Content.Text);
                }
            }


            Close();
        }
    }
}
