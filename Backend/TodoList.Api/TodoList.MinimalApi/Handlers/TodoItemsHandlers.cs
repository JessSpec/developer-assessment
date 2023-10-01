using AutoMapper;
using TodoList.Api.Models;
using TodoList.Core.Entities;
using TodoList.Core.Exceptions;
using TodoList.Core.Repositories;

namespace TodoList.MinimalApi.Handlers
{
    public static class TodoItemsHandlers
    {
        public static async Task<IResult> GetTodoItems(ITodoItemsRepository todoItemsRepository, IMapper mapper) 
        {
            var results = await todoItemsRepository.FindAllItems();
            return Results.Ok(mapper.Map<IList<TodoItem>, IList<TodoItemDto>>(results));
        }


        public static async Task<IResult> GetTodoItem(ITodoItemsRepository todoItemsRepository, IMapper mapper, int id)
        {
            var result = await todoItemsRepository.FindItemById(id);

            if (result == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(mapper.Map<TodoItem, TodoItemDto>(result));
        }


        public static async Task<IResult> PostTodoItem(ITodoItemsRepository todoItemsRepository, IMapper mapper, TodoItemDto todoItemDto)
        {
            if (string.IsNullOrEmpty(todoItemDto?.Description))
            {
                return Results.BadRequest("Description is required");
            }

            var todoItem = mapper.Map<TodoItemDto, TodoItem>(todoItemDto);
            try
            {
                await todoItemsRepository.AddItem(todoItem);

                return Results.Created($"/todoitems/{todoItem.Id}", todoItem);
            }
            catch (SaveTodoItemException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        public static async Task<IResult> PutTodoItem(ITodoItemsRepository todoItemsRepository, IMapper mapper, TodoItemDto todoItemDto, int id) 
        {
            if (id != todoItemDto.Id)
            {
                return Results.BadRequest();
            }

            var todoItem = mapper.Map<TodoItemDto, TodoItem>(todoItemDto);

            try
            {
                await todoItemsRepository.UpdateItem(id, todoItem);

                return Results.NoContent();
            }
            catch (SaveTodoItemException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
           
    }
}
