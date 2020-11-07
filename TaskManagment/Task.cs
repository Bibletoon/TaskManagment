using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagment
{
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
}
