using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskManagment
{
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
                        List<Task> tasks = Tmanager.AllTasks();
                        Console.Clear();
                        Console.WriteLine(Tmanager.GetTitle());
                        foreach (Task task in tasks)
                        {
                            Console.WriteLine(task.ToString());
                        }
                        Console.WriteLine();
                        break;
                    case "/completed":
                        List<Task> completed = Tmanager.CompletedTasks();
                        Console.Clear();
                        Console.WriteLine(Tmanager.GetTitle());
                        foreach (Task task in completed)
                        {
                            Console.WriteLine(task.ToString());
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