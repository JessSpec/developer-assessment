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
                if (!await TodoItemIdExists(id))
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

        public async Task<bool> TodoItemDescriptionExists(string description)
        {
            return await _context.TodoItems
                   .AnyAsync(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
        }

        private async Task<bool> TodoItemIdExists(int id)
        {
            return await _context.TodoItems.AnyAsync(x => x.Id == id);
        }       
    }
}
