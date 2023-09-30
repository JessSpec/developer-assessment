namespace TodoList.Api.Models
{
    //todo: worthwhile converting to record class??
    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
