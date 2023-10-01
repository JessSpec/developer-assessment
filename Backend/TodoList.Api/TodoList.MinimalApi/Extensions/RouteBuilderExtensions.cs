using Microsoft.AspNetCore.Builder;
using TodoList.MinimalApi.Handlers;

namespace TodoList.MinimalApi.Extensions
{
    public static class RouteBuilderExtensions
    {
        public static void RegisterTodoItemsEndpoints(this IEndpointRouteBuilder builder)
        {
            var todoItemsGroup = builder.MapGroup("/todoItems");

            todoItemsGroup.MapGet("", TodoItemsHandlers.GetTodoItems);
            todoItemsGroup.MapGet("/{id}", TodoItemsHandlers.GetTodoItem);
            todoItemsGroup.MapPost("/", TodoItemsHandlers.PostTodoItem);
            todoItemsGroup.MapPut("/{id}", TodoItemsHandlers.PutTodoItem);
        }
    }
}
