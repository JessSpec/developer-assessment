using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoList.Core.Contexts;
using TodoList.Core.Repositories;
using TodoList.MinimalApi.Extensions;

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
app.RegisterTodoItemsEndpoints();

app.Run();