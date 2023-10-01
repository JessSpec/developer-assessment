using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using TodoList.Api.Models;
using TodoList.Core.Entities;
using TodoList.Core.Exceptions;
using TodoList.Core.Repositories;

namespace TodoList.MinimalApi.Handlers
{
    public static class TodoItemsHandlers
    {
        public static async Task<Ok<IList<TodoItemDto>>> GetTodoItems(ITodoItemsRepository todoItemsRepository, IMapper mapper) 
        {
            var results = await todoItemsRepository.FindAllItems();
            return TypedResults.Ok(mapper.Map<IList<TodoItem>, IList<TodoItemDto>>(results));
        }

        public static async Task<Results<Ok<TodoItemDto>, NotFound>> GetTodoItem(ITodoItemsRepository todoItemsRepository, IMapper mapper, int id)
        {
            var result = await todoItemsRepository.FindItemById(id);

            if (result == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(mapper.Map<TodoItem, TodoItemDto>(result));
        }


        public static async Task<Results<Created<TodoItemDto>, BadRequest<string>>> PostTodoItem(ITodoItemsRepository todoItemsRepository, IMapper mapper, TodoItemDto todoItemDto)
        {
            if (string.IsNullOrEmpty(todoItemDto?.Description))
            {
                return TypedResults.BadRequest("Description is required");
            }

            var todoItem = mapper.Map<TodoItemDto, TodoItem>(todoItemDto);
            try
            {
                await todoItemsRepository.AddItem(todoItem);
                var itemDto = mapper.Map<TodoItem, TodoItemDto>(todoItem);

                return TypedResults.Created($"/todoitems/{todoItem.Id}", itemDto);
            }
            catch (SaveTodoItemException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        public static async Task<Results<BadRequest<string>, NoContent>> PutTodoItem(ITodoItemsRepository todoItemsRepository, IMapper mapper, TodoItemDto todoItemDto, int id) 
        {
            if (id != todoItemDto.Id)
            {
                return TypedResults.BadRequest("The id's do not match");
            }

            var todoItem = mapper.Map<TodoItemDto, TodoItem>(todoItemDto);

            try
            {
                await todoItemsRepository.UpdateItem(id, todoItem);

                return TypedResults.NoContent();
            }
            catch (SaveTodoItemException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }
           
    }
}
