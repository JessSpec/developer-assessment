using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Contexts;
using TodoList.Api.Entities;

namespace TodoList.Api.Repositories
{
    public class TodoItemsRepository : ITodoItemsRepository
    {
        private readonly TodoContext _context;
        public TodoItemsRepository(TodoContext context)
        {
            _context = context;
        }

        public async Task<IList<TodoItem>> FindAllItems()
        {
            return await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();
        }

        public async Task<TodoItem> FindItemById(int id)
        {
            //todo: no check for iscompleted?
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task UpdateItem(int id, TodoItem todoItem)
        {
            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //was just id
                if (!TodoItemIdExists(id))
                {
                    //throw specific exception?
                    //return null();

                }
                else
                {
                    throw;
                }
            }
        }

        public async Task AddItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
        }

        //todo: convert to async
        private bool TodoItemIdExists(int id)
        {
            return _context.TodoItems.Any(x => x.Id == id);
        }

        //todo: convert to async
        public bool TodoItemDescriptionExists(string description)
        {
            return _context.TodoItems
                   .Any(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
        }
    }
}
