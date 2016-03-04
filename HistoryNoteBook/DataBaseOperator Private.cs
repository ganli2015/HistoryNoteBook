using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace HistoryNoteBook
{
    public partial class DataBaseOperator
    {
        static int maxTagCount = 5;

        private DataTable QueryDataTable(string queryStr)
        {
            MySqlDataAdapter adapter = new MySqlDataAdapter(queryStr, _connection);
            DataTable data = new DataTable();
            adapter.Fill(data);

            return data;
        }

        /// <summary>
        /// Convert Data rows in table "event" to event list.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private List<Event> DataRowsToEventList(DataRowCollection rows)
        {
            List<Event> res = new List<Event>();
            foreach (DataRow row in rows)
            {
                Event ev = new Event();
                ev.Content = row["content"] as string;
                int year = (int)row["year"];
                int month = (int)row["month"];
                int day = (int)row["day"];
                int id = (int)row["id"];

                MyDateTime time = new MyDateTime(year, month, day);
                ev.Time = time;
                ev.ID = id;
                ev.Tags = GetTagList(row);

                res.Add(ev);
            }

            return res;
        }

        private List<Tag> GetTagList(DataRow row)
        {
            List<Tag> res = new List<Tag>();
            for (int i = 1; i <= maxTagCount;++i )
            {
                string tag = row["tag" + i.ToString()] as string;
                if(tag!="")
                {
                    res.Add(new Tag(tag));
                }
            }

            return res;
        }


        /// <summary>
        /// Convert data rows in table "edge" to id1&id2 pairs.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private List<KeyValuePair<int, int>> DataRowsToIDPairs(DataRowCollection rows)
        {
            List<KeyValuePair<int, int>> res = new List<KeyValuePair<int, int>>();
            foreach (DataRow row in rows)
            {
                int id1 = (int)row["id1"];
                int id2 = (int)row["id2"];

                res.Add(new KeyValuePair<int,int>(id1,id2));
            }

            return res;
        }

        private List<Tag> DataRowsToTags(DataRowCollection rows)
        {
            List<Tag> res = new List<Tag>();
            foreach (DataRow row in rows)
            {
                string str = (string)row["content"];

                res.Add(new Tag(str));
            }

            return res;
        }

        private List<KeyValuePair<int, int>> SearchAllIDPairs()
        {
            string str = "select * from edge";
            DataTable data = QueryDataTable(str);

            return DataRowsToIDPairs(data.Rows);
        }

        private void DeleteEdge(int id1,int id2)
        {
            string str = "delete from edge where id1=" + id1.ToString() + " and id2=" + id2.ToString();
            MySqlCommand command = new MySqlCommand(str, _connection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private List<Event> SelectDualEvents(List<KeyValuePair<int, int>> pairs, int myID)
        {
            List<Event> res = new List<Event>();
            foreach (KeyValuePair<int,int> p in pairs)
            {
                if (p.Key == myID && p.Value!=myID)
                {
                    Event ev = SeachEvents(p.Value);
                    if (ev != null)
                    {
                        res.Add(ev);
                    }
                }
                else if (p.Key != myID && p.Value == myID)
                {
                    Event ev = SeachEvents(p.Key);
                    if (ev != null)
                    {
                        res.Add(ev);
                    }
                }
            }

            return res;
        }

        private void Init()
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection("Data Source=localhost;Initial Catalog=HistoryNoteBook;User ID=root;Password=gggggg");
            }
            _connection.Open();
        }

        /// <summary>
        /// Get id of ev1 and ev2,and make earlier one id1 and the other id2.
        /// </summary>
        /// <param name="ev1"></param>
        /// <param name="ev2"></param>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        private void GetIDByTime(Event ev1, Event ev2, out int id1, out int id2)
        {
            if (ev1.Time <= ev2.Time)
            {
                id1 = ev1.ID;
                id2 = ev2.ID;
            }
            else
            {
                id1 = ev2.ID;
                id2 = ev1.ID;
            }
        }

        private bool EdgeExist(int id1, int id2)
        {
            string str= "select content from edge where id1=" + id1.ToString() + " and id2=" + id2.ToString();
            DataTable data = QueryDataTable(str);

            if (data.Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void PrepareTagParameter(Event ev,MySqlCommand command)
        {
            int tagCount = ev.Tags.Count;
            if (tagCount > maxTagCount)
            {
                throw new ArgumentOutOfRangeException("太多标签！");
            }

            //给标签赋值
            for (int i = 1; i <= tagCount;++i )
            {
                MySqlParameter tagPara = command.Parameters.Add("?tag"+i.ToString(), MySqlDbType.String);
                tagPara.Value = ev.Tags[i - 1].Text;
            }

            //多余的标签赋空值
            for (int i = tagCount + 1; i <= maxTagCount;++i )
            {
                MySqlParameter tagPara = command.Parameters.Add("?tag" + i.ToString(), MySqlDbType.String);
                tagPara.Value = null;
            }
        }

        private void AppendNewTag(List<Tag> tags)
        {
            List<Tag> allTags = SearchAllTag();
            tags.ForEach(t =>
            {
                if (allTags.Find(x => x.Text == t.Text) == null)
                {
                    InsertTag(t);
                }
            });
        }
    }
}
