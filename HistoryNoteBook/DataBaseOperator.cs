using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace HistoryNoteBook
{
    public partial class DataBaseOperator
    {
        static DataBaseOperator _instance;

        MySqlConnection _connection;

        private DataBaseOperator()
        {
            Init();
        }

        static public DataBaseOperator GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataBaseOperator();
            }

            return _instance;
        }

        public void InsertEvent(Event ev)
        {
            AppendNewTag(ev.Tags);

            MySqlParameter content, year, month,day;
            MySqlCommand command = new MySqlCommand("insert into event(content,year,month,day,tag1,tag2,tag3,tag4,tag5) values(?content,?year,?month,?day,?tag1,?tag2,?tag3,?tag4,?tag5)", _connection);
            content = command.Parameters.Add("?content", MySqlDbType.String);
            year = command.Parameters.Add("?year", MySqlDbType.Int32);
            month = command.Parameters.Add("?month", MySqlDbType.Int16);
            day = command.Parameters.Add("?day", MySqlDbType.Int16);
            PrepareTagParameter(ev, command);
            command.Prepare();

            content.Value = ev.Content;
            year.Value = ev.Year;
            month.Value = ev.Time.Month;
            day.Value = ev.Time.Day;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<Event> SeachEventsOfOneYear(int eventYear)
        {
            string str="select * from event where year=" + eventYear.ToString();
            DataTable data=QueryDataTable(str);

            return DataRowsToEventList(data.Rows);
        }

        public Event SeachEvents(int id)
        {
            string str = "select * from event where id=" + id.ToString();
            DataTable data = QueryDataTable(str);
            List<Event> evs=DataRowsToEventList(data.Rows);

            if (evs.Count == 1)
            {
                return evs[0];
            }
            else
            {
                return null;
            }
        }

        public List<Event> SeachEvents(string content)
        {
            string str = "select * from event where content=" + "'"+content+"'";
            DataTable data = QueryDataTable(str);

            return DataRowsToEventList(data.Rows);
        }

        public List<Event> SearchEvents_ContainStr(string content)
        {
            string str = "select * from event where content like " + "'%" + content + "%'";
            DataTable data = QueryDataTable(str);

            return DataRowsToEventList(data.Rows);
        }

        public List<Event> SearchAllEvents()
        {
            string str = "select * from event";
            DataTable data = QueryDataTable(str);

            return DataRowsToEventList(data.Rows);
        }

        public List<Event> SearchAdjacentEvents(Event ev)
        {
            int id = ev.ID;
            if (ev.ID == -1)
            {
                id = GetEventID(ev);
            }
            if (id == -1)//No such event
            {
                return null;
            }

            string str = "select * from edge where id1="+id.ToString()+" or id2="+id.ToString();
            DataTable data = QueryDataTable(str);
            List<KeyValuePair<int, int>> idPair = DataRowsToIDPairs(data.Rows);

            return SelectDualEvents(idPair, id);
        }

        public int GetEventID(Event ev)
        {
            List<Event> evs = SeachEvents(ev.Content);
            if (evs.Count == 1)
            {
                return evs[0].ID;
            }

            return -1;
        }

        public void UpdateEvent(Event ev)
        {
            AppendNewTag(ev.Tags);

            MySqlCommand updateCommond = new MySqlCommand("update event set content=?content,year=?year,month=?month,day=?day,tag1=?tag1,tag2=?tag2,tag3=?tag3,tag4=?tag4,tag5=?tag5 where id=" + ev.ID.ToString(), _connection);
            MySqlParameter content, year, month, day;
            content=updateCommond.Parameters.Add("?content", MySqlDbType.String);
            year=updateCommond.Parameters.Add("?year", MySqlDbType.Int32);
            month=updateCommond.Parameters.Add("?month", MySqlDbType.Int16);
            day=updateCommond.Parameters.Add("?day", MySqlDbType.Int16);
            PrepareTagParameter(ev, updateCommond);

            content.Value = ev.Content;
            year.Value = ev.Year;
            month.Value = ev.Month;
            day.Value = ev.Day;

            updateCommond.ExecuteNonQuery();
        }

        public void InsertEdge(Event ev1, Event ev2,string desc)
        {
            //Make the event of id1 earlier than one of id2.
            int id1, id2;
            GetIDByTime(ev1, ev2, out id1, out id2);
            if (EdgeExist(id1, id2) || id1 == -1 || id2 == -1)
            {
                return;
            }

            MySqlParameter content, id1Param, id2Param;
            MySqlCommand command = new MySqlCommand("insert into edge(id1,id2,content) values(?id1,?id2,?content)", _connection);
            content = command.Parameters.Add("?content", MySqlDbType.String);
            id1Param = command.Parameters.Add("?id1", MySqlDbType.Int64);
            id2Param = command.Parameters.Add("?id2", MySqlDbType.Int64);

            id1Param.Value = id1;
            id2Param.Value = id2;
            content.Value = desc;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string SearchEdgeInfo(Event ev1, Event ev2)
        {
            int id1 = ev1.ID;
            int id2 = ev2.ID;
            string commandStr = "select content from edge where id1=" + id1.ToString() + " and id2=" + id2.ToString();
            DataTable data = QueryDataTable(commandStr);

            if (data.Rows.Count > 0)
            {
                return data.Rows[0]["content"] as string;
            }
            else
            {
                return "";
            }

        }


        public bool EdgeExist(Event ev1, Event ev2)
        {
            int id1, id2;
            GetIDByTime(ev1, ev2, out id1, out id2);
            if (EdgeExist(id1, id2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DeleteEdge(Event ev1, Event ev2)
        {
            int id1, id2;
            GetIDByTime(ev1, ev2, out id1, out id2);

            DeleteEdge(id1, id2);
        }

        public void RemoveDuplicatedEdges()
        {
            List<KeyValuePair<int, int>> allPairs = SearchAllIDPairs();
            foreach(KeyValuePair<int,int> pair in allPairs)
            {
                int id1 = pair.Key;
                int id2 = pair.Value;
                KeyValuePair<int, int> reverse = new KeyValuePair<int, int>(id2, id1);
                if (allPairs.Contains(reverse))
                {
                    Event ev1 = SeachEvents(id1);
                    Event ev2 = SeachEvents(id2);
                    if (ev1.Time < ev2.Time)
                    {
                        DeleteEdge(id2, id1);
                    }
                    else
                    {
                        DeleteEdge(id1, id2);
                    }
                }
            }
        }

        public void OutputTableToFile(string tablename, string filename)
        {
            string str = "select * from "+tablename+" into outfile '"+filename+"' fields terminated by ','";
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

        public List<Tag> SearchAllTag()
        {
            string str = "select * from tag";
            DataTable data = QueryDataTable(str);

            return DataRowsToTags(data.Rows);
        }

        public void InsertTag(Tag tag)
        {
            MySqlCommand command = new MySqlCommand("insert into tag(content) values(?content)", _connection);
            MySqlParameter content = command.Parameters.Add("?content", MySqlDbType.String);
            content.Value = tag.Text;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool TagExist(Tag tag)
        {
            string str = "select * from tag where content='" + tag.Text+"'";
            DataTable data = QueryDataTable(str);
            List<Tag> evs = DataRowsToTags(data.Rows);

            if (evs.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
