using Kanban.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = Kanban.Models.Task;

namespace Kanban.Data
{
    public class SqlContext: DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        {

        }
        public DbSet<Task> Task { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Status> Status { get; set; }



    



    }
}
