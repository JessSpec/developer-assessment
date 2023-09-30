using Microsoft.EntityFrameworkCore;
using TodoList.Core.Entities;

namespace TodoList.Core.Contexts
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
