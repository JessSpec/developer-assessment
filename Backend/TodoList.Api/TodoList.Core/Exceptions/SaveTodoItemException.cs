namespace TodoList.Core.Exceptions
{
    public class SaveTodoItemException : Exception
    {
        public SaveTodoItemException(string? message) : base(message)
        {
        }
    }
}
