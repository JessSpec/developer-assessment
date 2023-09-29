using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Api.Entities
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}
