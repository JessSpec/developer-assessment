using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Core.Entities
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        //todo: enforce uniqueness at a db level?
        public string Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}
