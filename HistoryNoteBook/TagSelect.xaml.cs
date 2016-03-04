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
    /// TagSelect.xaml 的交互逻辑
    /// </summary>
    public partial class TagSelect : Window
    {
        BuildTagTreeHandler _handler;
        Event _ev;
        public TagSelect(Event ev, BuildTagTreeHandler handler)
        {
            InitializeComponent();
            _handler = handler;
            _ev = ev;

            ev.Tags.ForEach(t =>
            {
                if (t.Text != null)
                {
                    CheckBox cb = new CheckBox();
                    cb.Content = t.Text;
                    cb.Margin = new System.Windows.Thickness(5);
                    cb.HorizontalAlignment = HorizontalAlignment.Center;
                    stackPanel1.Children.Add(cb);
                }
            });
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            List<string> tags=new List<string>();
            foreach(CheckBox cb in stackPanel1.Children)
            {
                if(cb.IsChecked==true)
                {
                    tags.Add(cb.Content as string);
                }
            }

            _handler(_ev, tags);
        }
    }
}
