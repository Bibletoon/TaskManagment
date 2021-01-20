using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace TaskManagment
{
    class ApplicationContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> optionsBuilder) : base(optionsBuilder)
        {
            Database.EnsureCreated();
        }
    }
}
