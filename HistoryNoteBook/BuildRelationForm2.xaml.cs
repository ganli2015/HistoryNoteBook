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
    /// BuildRelationForm2.xaml 的交互逻辑
    /// </summary>
    public partial class BuildRelationForm2 : Window
    {
        Event _ev1;
        Event _ev2;
        MakeEdgeHandler _makeEdgeHandler;

        public BuildRelationForm2(Event ev1, Event ev2, MakeEdgeHandler makeEdgeHandler)
        {
            InitializeComponent();

            _ev1 = ev1;
            _ev2 = ev2;
            _makeEdgeHandler = makeEdgeHandler;
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            _makeEdgeHandler(_ev1, _ev2, textBox_Content.Text);

            Close();
        }
    }
}
