using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TaskManagment
{
    class TaskManager
    {
        private static TaskManager Instance;

        private List<Task> Tasks;

        public static TaskManager GetInstance() => Instance ?? (Instance = new TaskManager());

        public TaskManager()
        {
            DataBaseManager manager = DataBaseManager.GetInstance();
            Tasks = manager.LoadTasks();
        }

        public void ShowTitle()
        {
            Console.Clear();
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
            long id = manager.WriteTask(title, status);
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
                outputFile.WriteLine($"{task.Status % 2}{task.Title}");
            }
            outputFile.Close();
        }

        public void LoadFromFile(string filename)
        {
            StreamReader inputFile = new StreamReader(filename);
            string taskString;
            while ((taskString = inputFile.ReadLine()) != null && taskString != "")
            {
                int newStatus = taskString[0] - '0';
                AddTask(taskString.Substring(1), newStatus);
            }
        }
    }

}
