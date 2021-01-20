using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManagment.Enums;


namespace TaskManagment
{
    class DataBaseManager
    {
        private static DataBaseManager Instance;

        private DbContextOptions<ApplicationContext> options;

        public DataBaseManager()
        {
            ConfigureConnection();
        }

        public static DataBaseManager GetInstance() => Instance ?? (Instance = new DataBaseManager());

        public void ConfigureConnection()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            options = optionsBuilder.UseSqlite(connectionString).Options;
        }

        public List<Task> LoadTasks()
        {
            using (ApplicationContext db = new ApplicationContext(options))
            {
                return db.Tasks.ToList();
            }
        }

        public long WriteTask(string title, StatusCode status)
        {
            using (ApplicationContext db = new ApplicationContext(options))
            {
                var newTask = new Task(title,status);
                db.Tasks.Add(newTask);
                db.SaveChanges();
                return newTask.Id;
            }
        }

        public void CompleteTask(long id)
        {
            using (ApplicationContext db = new ApplicationContext(options))
            {
                var task = db.Tasks.Find(id);
                task.Status = StatusCode.Complited;
                db.SaveChanges();
            }
        }

        public void DeleteTask(long id)
        {
            using (ApplicationContext db = new ApplicationContext(options))
            {
                var task = db.Tasks.Find(id);
                db.Tasks.Remove(task);
                db.SaveChanges();
            }
        }
    }
}
