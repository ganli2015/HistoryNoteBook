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
    /// TagEditor.xaml 的交互逻辑
    /// </summary>
    public partial class TagEditor : Window
    {
        private UpdateTagHander _updateTagHander;
        private List<Tag> _tagInList;

        public TagEditor(string eventContent,List<Tag> tagInList,UpdateTagHander updateTagHander)
        {
            InitializeComponent();

            _updateTagHander = updateTagHander;
            _tagInList = tagInList;
            textBox_Content.Text = SearchPossibleTag(eventContent);
            textBox_Content.Focus();
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            _updateTagHander(textBox_Content.Text);
            Close();
        }

        private string SearchPossibleTag(string eventContent)
        {
            string charNum3 = SearchPossibleTag(eventContent, 3);
            if (charNum3 != "")
            {
                return charNum3;
            }

            return SearchPossibleTag(eventContent, 2);
        }

        /// <summary>
        /// search tag whose number of character is <charNum>
        /// </summary>
        /// <param name="eventContent"></param>
        /// <param name="charNum"></param>
        /// <returns></returns>
        private string SearchPossibleTag(string eventContent,int charNum)
        {
            for (int i = 0; i < eventContent.Length - charNum+1;++i )
            {
                //获得连续charNum个字符串
                string possibleTag = "";
                for (int j = 0; j < charNum;++j )
                {
                    possibleTag += eventContent[i + j];
                }

                Tag tag = new Tag(possibleTag);
                if (DataBaseOperator.GetInstance().TagExist(tag))
                {
                    if (_tagInList.Find(x=>x.Text==tag.Text)==null)
                    {
                        return possibleTag;

                    }
                }
            }

            return "";
        }
    }
}
