using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagment
{
    enum StatusCode
    {
        Uncomplited,
        Complited
    }

    class Task
    {
        public long Id;
        public string Title;
        public StatusCode Status;

        public Task(long id, string title, StatusCode status)
        {
            Title = title;
            Id = id;
            Status = status;
        }

        public override string ToString()
        {
            return String.Format("{0,3}{1,15}{2,14}", Id, Title,
                Status == 0 ? "uncompleted" : "completed");
        }
    }
}
