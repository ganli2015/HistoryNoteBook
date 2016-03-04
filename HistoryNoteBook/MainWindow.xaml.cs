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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace HistoryNoteBook
{
    public delegate void UpdateMainHandler(Event ev);
    public delegate void MakeEdgeHandler(Event ev1,Event ev2,string desc);
    public delegate void BuildTagTreeHandler(Event ev,List<string> tags);

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        DataBaseOperator _databaseOperator;

        public MainWindow()
        {
            InitializeComponent();
            _databaseOperator = DataBaseOperator.GetInstance();
        }

        private void AddEventToDataBase(Event ev)
        {
            _databaseOperator.InsertEvent(ev);

            Search();
        }

        private void UpdateEventToDataBase(Event ev)
        {
            _databaseOperator.UpdateEvent(ev);

            Search();
        }

        private void MakeEdge(Event ev1, Event ev2,string desc)
        {
            _databaseOperator.InsertEdge(ev1, ev2, desc);
        }

        private void button_Search_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            int selected = comboBox_Filter.SelectedIndex;
            List<Event> events = null;
            switch (selected)
            {
                case 0:
                    {
                        events=SearchByYear();
                        break;
                    }
                case 1:
                    {
                        events=SearchByYearSpan();
                        break;
                    }
                case 2:
                    {
                        events=SearchByContent();
                        break;
                    }
            }

            listBox_Events.Items.Clear();
            if (events != null)
            {
                AppendEventsToListbox(events);
            }
        }

        private void textBox_Year_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search();
            }
        }

        private List<Event> SearchByYear()
        {
            int year;
            if (!int.TryParse(textBox_Search.Text, out year))
            {
                return null;
            }

            return _databaseOperator.SeachEventsOfOneYear(year);
        }

        private List<Event> SearchByYearSpan()
        {
            List<string> split = CommonFunction.Split(textBox_Search.Text, ' ');
            if (split.Count != 2) return null;

            int yearFrom;
            if (!int.TryParse(split[0], out yearFrom))
            {
                return null;
            }
            int yearTo;
            if (!int.TryParse(split[1], out yearTo))
            {
                return null;
            }
            if (yearTo < yearFrom) return null;

            List<Event> res = new List<Event>();
            for (int i = yearFrom; i < yearTo;++i )
            {
                List<Event> events = _databaseOperator.SeachEventsOfOneYear(i);
                res.AddRange(events);
            }

            return res;
        }

        private List<Event> SearchByContent()
        {
            return _databaseOperator.SearchEvents_ContainStr(textBox_Search.Text);
        }

        private void AppendEventsToListbox(List<Event> events)
        {
            CommonFunction.SortEvents(events);
            events.ForEach(ev =>
            {
                listBox_Events.Items.Add(CommonFunction.CreateTextBlockUI(ev.Content, listBox_Events.Width * 0.98));
            });
        }

        private void Button_NewEvent_Click(object sender, RoutedEventArgs e)
        {
            EventEditor input = new EventEditor(null, new UpdateMainHandler(AddEventToDataBase));
            input.Title = Button_NewEvent.Content as string;
            input.Owner = this;
            input.ShowDialog();

        }

        private void listBox_Events_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(listBox_Events.SelectedItem==null) return;

            string content = CommonFunction.GetContent(listBox_Events.SelectedItem);
            List<Event> events = _databaseOperator.SeachEvents(content);
            if (events.Count != 1)
            {
                return;
            }

            Event ev=events[0];
            EventEditor input = new EventEditor(ev,new UpdateMainHandler(UpdateEventToDataBase));
            input.Owner = this;
            input.ShowDialog();
        }

        

        private void listBox_Events_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (listBox_Events.SelectedItem == null) return;

            string content = CommonFunction.GetContent(listBox_Events.SelectedItem);
            Event ev = GetEvent(content);
            if (ev == null)
            {
                MessageBox.Show("搜不到事件！");
                return;
            }

            BuildRelationForm buildRelationForm = new BuildRelationForm();
            buildRelationForm.databaseOperator = _databaseOperator;
            buildRelationForm.firstEvent = ev;
            buildRelationForm._makeEdgeHandler += new MakeEdgeHandler(MakeEdge);
            buildRelationForm.Owner = this;
            buildRelationForm.ShowDialog();
        }

        private Event GetEvent(string content)
        {
            List<Event> events = _databaseOperator.SeachEvents(content);
            if (events.Count != 1)
            {
                return null;
            }

            return events[0];
        }

        private void button_ShowAllEvents_Click(object sender, RoutedEventArgs e)
        {
            AllEventDisplayForm form = new AllEventDisplayForm(_databaseOperator);
            form.Owner = this;
            form.ShowDialog();
        }

        private void button_BuildGraph_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_Events.SelectedItem == null) return;

            string content = CommonFunction.GetContent(listBox_Events.SelectedItem);
            Event ev = GetEvent(content);
            if (ev == null)
            {
                MessageBox.Show("搜不到事件！");
                return;
            }

            EventGraphForm form = new EventGraphForm(ev);
            form.Owner = this;
            form.ShowDialog();
        }

        private void button_BuildRelation_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_Events.SelectedItems.Count != 2) return;

            string content1 = CommonFunction.GetContent(listBox_Events.SelectedItems[0]);
            Event ev1 = GetEvent(content1);
            string content2 = CommonFunction.GetContent(listBox_Events.SelectedItems[1]);
            Event ev2 = GetEvent(content2);
            if (ev1 == null || ev2 == null) return;

            BuildRelationForm2 form = new BuildRelationForm2(ev1, ev2, MakeEdge);
            form.Owner = this;
            form.ShowDialog();
        }

        private void button_Backup_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("确定备份？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            string databaseDir=@"C:\ProgramData\MySQL\MySQL Server 5.6\data\historynotebook\";

            //output table
            string edgeFile=databaseDir + "edge.csv";
            string eventFile=databaseDir + "event.csv";
            if (File.Exists(edgeFile))
            {
                File.Delete(edgeFile);
            }
            if (File.Exists(eventFile))
            {
                File.Delete(eventFile);
            }
            _databaseOperator.OutputTableToFile("event", "event.csv");
            _databaseOperator.OutputTableToFile("edge", "edge.csv");

            //copy directory
            string mydataDir = @"data\";
            if (!Directory.Exists(mydataDir))
            {
                Directory.CreateDirectory(mydataDir);
            }
            
            string[] fn = Directory.GetFiles(databaseDir);
            foreach (string path in fn)
            {
                string filename = System.IO.Path.GetFileName(path);
                if (File.Exists(mydataDir + filename))
                {
                    File.Delete(mydataDir + filename);
                }
                File.Copy(path, mydataDir + filename);
            }
        }

        private void button_RemoveEdge_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("确定去除冗余关系？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            _databaseOperator.RemoveDuplicatedEdges();
        }

        private void BuildTagTree(Event ev,List<string> Tags)
        {
            EventGraphForm form = new EventGraphForm(ev,Tags);
            form.Owner = this;
            form.ShowDialog();
        }

        private void button_BuildTagTree_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_Events.SelectedItem == null) return;

            string content = CommonFunction.GetContent(listBox_Events.SelectedItem);
            Event ev = GetEvent(content);
            if (ev == null)
            {
                MessageBox.Show("搜不到事件！");
                return;
            }

            TagSelect form = new TagSelect(ev,BuildTagTree);
            form.Owner = this;
            form.ShowDialog();
        }

        
    }
}
