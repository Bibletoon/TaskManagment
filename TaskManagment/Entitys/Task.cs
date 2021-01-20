using System;
using System.Collections.Generic;
using System.Text;
using TaskManagment.Enums;

namespace TaskManagment
{
    class Task
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public StatusCode Status { get; set; }

        public Task(long id, string title, StatusCode status)
        {
            Title = title;
            Id = id;
            Status = status;
        }

        public Task(string title, StatusCode status)
        {
            Title = title;
            Status = status;
        }

        public override string ToString()
        {
            return String.Format("{0,3}{1,15}{2,14}", Id, Title,
                Status == 0 ? "uncompleted" : "completed");
        }
    }
}
