using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskManagment.Enums;

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

        public string GetTitle()
        {
            return String.Format("{0,3}{1,15}{2,14}", "Id", "Title", "Status");
        }

        public List<Task> AllTasks()
        {
            return Tasks;
        }

        public List<Task> CompletedTasks()
        {
            return Tasks.FindAll(n => n.Status != 0);
        }

        public Task AddTask(string title, StatusCode status = StatusCode.Uncomplited)
        {
            if (Tasks.Any(n=>n.Title==title))
            {
                return null;
            }

            DataBaseManager manager = DataBaseManager.GetInstance();
            long id = manager.WriteTask(title, status);
            Task newTask = new Task(id, title, status);
            Tasks.Add(newTask);
            return newTask;
        }

        public void CompleteTask(long id)
        {
            var res = from task in Tasks where task.Id == id select task;
            res.First().Status = StatusCode.Complited;
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
            using StreamWriter outputFile = new StreamWriter(filename);
            foreach (Task task in Tasks)
            {
                outputFile.WriteLine($"{(int)task.Status}{task.Title}");
            }
        }

        public void LoadFromFile(string filename)
        {
            using StreamReader inputFile = new StreamReader(filename);
            string taskString;
            while ((taskString = inputFile.ReadLine()) != null && taskString != "")
            {
                StatusCode newStatus = taskString[0] == '0' ? StatusCode.Uncomplited : StatusCode.Complited;
                AddTask(taskString.Substring(1), newStatus);
            }
        }
    }

}
