using TodoList.Core.Entities;

namespace TodoList.Core.Repositories
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