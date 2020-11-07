using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

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

        public List<(long, string, int)> LoadTasks()
        {
            string loadQuery = "SELECT * FROM Tasks";
            SQLiteCommand loadCommand = new SQLiteCommand(loadQuery, Connection);
            Connection.Open();
            SQLiteDataReader sqlReader = loadCommand.ExecuteReader();
            List<(long, string, int)> result = new List<(long, string, int)>();
            while (sqlReader.Read())
            {
                result.Add(((long) sqlReader["ID"], (string) sqlReader["Title"],
                    Int32.Parse(sqlReader["Status"].ToString())));
            }

            Connection.Close();
            return result;
        }

        public long WriteTask(string title,int status)
        {
            string writeQuery = "INSERT INTO Tasks (ID,Title,Status) VALUES (NULL,@TITLE,@STATUS);";
            SQLiteCommand writeCommand = new SQLiteCommand(writeQuery, Connection);
            writeCommand.Parameters.Add(new SQLiteParameter("@TITLE", title));
            writeCommand.Parameters.Add(new SQLiteParameter("@STATUS", status));
            Connection.Open();
            writeCommand.ExecuteNonQuery();

            string GetQuery = "SELECT last_insert_rowid()";
            SQLiteCommand GetCommand = new SQLiteCommand(GetQuery, Connection);

            long id = (long) GetCommand.ExecuteScalar();
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
            SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery,Connection);
            deleteCommand.Parameters.Add(new SQLiteParameter("@Id", id));
            Connection.Open();
            deleteCommand.ExecuteNonQuery();
            Connection.Close();
        }
    }

    class TaskManager
    {
        private static TaskManager Instance;

        private List<Task> Tasks;

        public static TaskManager GetInstance() => Instance ?? (Instance = new TaskManager());

        public TaskManager()
        {
            DataBaseManager manager = DataBaseManager.GetInstance();
            Tasks = new List<Task>();
            foreach (var (id, title, status) in manager.LoadTasks())
            {
                Task newTask = new Task(id, title, status);
                Tasks.Add(newTask);
            }
        }

        public void ShowTitle()
        {
            Console.WriteLine("{0,3}{1,15}{2,14}", "Id", "Title", "Status");
        }

        public List<Task> AllTasks()
        {
            return Tasks;
        }
        
        public List<Task> CompletedTasks()
        {
            return Tasks.FindAll(n => n.Status != 0);
        }

        public void AddTask(string title, int status = 0)
        {
            if (Tasks.Count(task => task.Title == title) != 0)
            {
                return;
            }

            DataBaseManager manager = DataBaseManager.GetInstance();
            long id = manager.WriteTask(title,status);
            Task newTask = new Task(id, title, status);
            Tasks.Add(newTask);
        }

        public void CompleteTask(long id)
        {
            var res = from task in Tasks where task.Id == id select task;
            res.First().Status = 1;
            DataBaseManager manager = DataBaseManager.GetInstance();
            manager.CompleteTask(id);
        }

        public void DeleteTask(long id)
        {
            DataBaseManager manager = DataBaseManager.GetInstance();
            manager.DeleteTask(id);
            Tasks.Remove(Tasks.Find(n => n.Id == id));
        }

        public void SaveToFile(string filename)
        {
            StreamWriter outputFile = new StreamWriter(filename);
            foreach (Task task in Tasks)
            {
                outputFile.WriteLine($"{task.Status%2}{task.Title}");       
            }
            outputFile.Close();
        }

        public void LoadFromFile(string filename)
        {
            StreamReader inputFile = new StreamReader(filename);
            string taskString;
            while ((taskString=inputFile.ReadLine())!=null && taskString!="")
            {
                int newStatus = taskString[0]-'0';
                AddTask(taskString.Substring(1), newStatus);
            }
        }
    }

    class Task
    {
        public long Id;
        public string Title;
        public int Status;

        public Task(long id, string title, int status)
        {
            Title = title;
            Id = id;
            Status = status;
        }

        public void Show()
        {
            Console.WriteLine("{0,3}{1,15}{2,14}", Id, Title,
                Status == 0 ? "uncompleted" : "completed");
        }
    }

    class Program
    {
        private static void Main(string[] args)
        {
            string[] command;
            string request;
            TaskManager Tmanager = TaskManager.GetInstance();
            bool end = false;
            long id;
            while (!end && (request = Console.ReadLine()) != null && request != "")
            {
                command = request.Split();
                switch (command[0])
                {
                    case "/add":
                        Tmanager.AddTask(String.Join(" ", command.Skip(1)));
                        Console.WriteLine($"Task \"{String.Join(" ", command.Skip(1))}\" added.");
                        break;

                    case "/all":
                        Console.Clear();
                        List<Task> tasks = Tmanager.AllTasks();
                        Tmanager.ShowTitle();
                        foreach (Task task in tasks)
                        {
                            task.Show();
                        }
                        Console.WriteLine();
                        break;
                    case "/completed":
                        List<Task> completed = Tmanager.CompletedTasks();
                        Tmanager.ShowTitle();
                        foreach (Task task in completed)
                        {
                            task.Show();
                        }
                        break;
                    case "/complete":
                        id = Int64.Parse(command[1]);
                        Tmanager.CompleteTask(id);
                        Console.WriteLine($"Task #{id} marked as completed.");
                        break;
                    case "/delete":
                        id = Int64.Parse(command[1]);
                        Tmanager.DeleteTask(id);
                        Console.WriteLine($"Task #{id} is deleted.");
                        break;
                    case "/save":
                        Tmanager.SaveToFile(command[1]);
                        Console.WriteLine($"Tasks saved to file {command[1]}.");
                        break;
                    case "/load":
                        Tmanager.LoadFromFile(command[1]);
                        Console.WriteLine("Tasks loaded from file.");
                        break;
                    case "/exit":
                        end = true;
                        break;
                    case "/help":
                        Console.Clear();
                        Console.WriteLine("/add <task-info> - Add new task");
                        Console.WriteLine("/all - Show all tasks");
                        Console.WriteLine("/delete <id> - Delete task by id");
                        Console.WriteLine("/save <filename> - Save all tasks to file");
                        Console.WriteLine("/load <filename> - Load tasks from file");
                        Console.WriteLine("/complete <id> - Mark task as completed by id");
                        Console.WriteLine("/completed - Show all completed tasks");
                        Console.WriteLine();
                        break;
                    default:
                        Console.WriteLine("Wrong command! Type /help to see list of commands.");
                        break;
                }
            }
        }
    }
}