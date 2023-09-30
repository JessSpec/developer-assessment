using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Api.Entities;

namespace TodoList.Api.Repositories
{
    public interface ITodoItemsRepository
    {
        Task AddItem(TodoItem todoItem);
        Task<IList<TodoItem>> FindAllItems();
        Task<TodoItem> FindItemById(int id);
        Task UpdateItem(int id, TodoItem todoItem);
        Task<bool> TodoItemDescriptionExists(string description);
    }
}