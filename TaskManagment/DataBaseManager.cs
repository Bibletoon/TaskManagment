using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace TaskManagment
{
    class DataBaseManager
    {
        private static DataBaseManager Instance;

        private SQLiteConnection Connection;

        public DataBaseManager()
        {
            ConfigureConnection();
        }

        public static DataBaseManager GetInstance() => Instance ?? (Instance = new DataBaseManager());

        private void ConfigureConnection()
        {
            if (!File.Exists(@"./Tasks.db"))
            {
                SQLiteConnection.CreateFile("./Tasks.db");
            }

            Connection = new SQLiteConnection("Data Source = ./Tasks.db");
            CheckTable();
        }

        private void CheckTable()
        {
            string createQuery =
                "CREATE TABLE IF NOT EXISTS Tasks (ID INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, Title TEXT, Status INTEGER DEFAULT 0)";
            SQLiteCommand CreateCommand = new SQLiteCommand(createQuery, Connection);
            Connection.Open();
            CreateCommand.ExecuteNonQuery();
            Connection.Close();
        }

        public List<Task> LoadTasks()
        {
            string loadQuery = "SELECT * FROM Tasks";
            SQLiteCommand loadCommand = new SQLiteCommand(loadQuery, Connection);
            Connection.Open();
            SQLiteDataReader sqlReader = loadCommand.ExecuteReader();
            List<Task> result = new List<Task>();
            while (sqlReader.Read())
            {
                result.Add(new Task((long)sqlReader["ID"], (string)sqlReader["Title"],
                    Int32.Parse(sqlReader["Status"].ToString())));
            }

            Connection.Close();
            return result;
        }

        public long WriteTask(string title, int status)
        {
            string writeQuery = "INSERT INTO Tasks (ID,Title,Status) VALUES (NULL,@TITLE,@STATUS);";
            SQLiteCommand writeCommand = new SQLiteCommand(writeQuery, Connection);
            writeCommand.Parameters.Add(new SQLiteParameter("@TITLE", title));
            writeCommand.Parameters.Add(new SQLiteParameter("@STATUS", status));
            Connection.Open();
            writeCommand.ExecuteNonQuery();

            string GetQuery = "SELECT last_insert_rowid()";
            SQLiteCommand GetCommand = new SQLiteCommand(GetQuery, Connection);

            long id = (long)GetCommand.ExecuteScalar();
            Connection.Close();

            return id;
        }

        public void CompleteTask(long id)
        {
            string UpdateQuery = "UPDATE Tasks SET Status=1 WHERE id=" + id.ToString();
            SQLiteCommand updateCommand = new SQLiteCommand(UpdateQuery, Connection);
            Connection.Open();
            updateCommand.ExecuteNonQuery();
            Connection.Close();
        }

        public void DeleteTask(long id)
        {
            string deleteQuery = "DELETE FROM Tasks WHERE ID = @Id";
            SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, Connection);
            deleteCommand.Parameters.Add(new SQLiteParameter("@Id", id));
            Connection.Open();
            deleteCommand.ExecuteNonQuery();
            Connection.Close();
        }
    }
}
