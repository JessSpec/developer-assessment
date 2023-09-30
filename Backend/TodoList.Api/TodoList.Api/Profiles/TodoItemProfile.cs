using AutoMapper;
using TodoList.Core.Entities;
using TodoList.Api.Models;

namespace TodoList.Api.Profiles
{
    public class TodoItemProfile : Profile
    {
        public TodoItemProfile()
        {
            CreateMap<TodoItem, TodoItemDto>().ReverseMap();
        }
    }
}
