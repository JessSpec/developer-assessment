using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoList.Api.Models;
using TodoList.Core.Contexts;
using TodoList.Core.Entities;
using TodoList.Core.Exceptions;
using TodoList.Core.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoList.MinimalApi", Version = "v1" });
});

builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoItemsDB"));
builder.Services.AddTransient<ITodoItemsRepository, TodoItemsRepository>();

//todo: cors

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/todoItems", async (ITodoItemsRepository todoItemsRepository, IMapper mapper) =>
{
    var results = await todoItemsRepository.FindAllItems();
    return Results.Ok(mapper.Map<IList<TodoItem>, IList<TodoItemDto>>(results));
});

app.MapGet("/todoItems/{id}", async (ITodoItemsRepository todoItemsRepository, IMapper mapper, int id) =>
{
    var result = await todoItemsRepository.FindItemById(id);

    if (result == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(mapper.Map<TodoItem, TodoItemDto>(result));
});

app.MapPost("/todoItems", async (ITodoItemsRepository todoItemsRepository, IMapper mapper, TodoItemDto todoItemDto) =>
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

});

app.MapPut("/todoItems/{id}", async (ITodoItemsRepository todoItemsRepository, IMapper mapper, TodoItemDto todoItemDto, int id) =>
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
    catch(SaveTodoItemException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    
});

app.Run();