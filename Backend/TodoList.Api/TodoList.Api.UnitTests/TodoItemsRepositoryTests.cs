using AutoMapper;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Core.Contexts;
using TodoList.Core.Entities;
using TodoList.Core.Exceptions;
using TodoList.Core.Repositories;
using Xunit;

namespace TodoList.Api.UnitTests
{
    public class TodoItemsRepositoryTests
    {
        static TodoItemsRepository _todoItemsRepository;
        static List<TodoItem> _inMemoryList;

        static TodoItemsRepositoryTests()
        {
            _inMemoryList = new List<TodoItem>()
            {
                new TodoItem() {Id = 1, Description = "Refactor", IsCompleted = false },
                new TodoItem() {Id = 2, Description = "Test", IsCompleted = false },
                new TodoItem() {Id = 3, Description = "Do something", IsCompleted = true}
            };

            var contextMock = CreateMockContext();
            var loggerMock = Substitute.For<ILogger<ITodoItemsRepository>>();

            _todoItemsRepository = new TodoItemsRepository(contextMock, loggerMock);

        }

        static TodoContext CreateMockContext()
        {
            var dbContextOptions = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase("TodoItemsTestDB")
            .Options;

            TodoContext context = new TodoContext(dbContextOptions);

            //populate context
            context.AddRange(_inMemoryList);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task FindAll_Should_ReturnItems()
        {
            var items = await _todoItemsRepository.FindAllItems();

            Assert.Equal(2, items.Count);
        }

        [Fact]
        public async Task FindItemById_Should_ReturnItem()
        {
            var item = await _todoItemsRepository.FindItemById(2);

            Assert.NotNull(item);
            Assert.Equal(2, item.Id);
        }

        [Fact]
        public async Task FindItemById_Should_ReturnNull_IfNotExists()
        {
            var item = await _todoItemsRepository.FindItemById(99);

            Assert.Null(item);
        }

        [Fact]
        public async Task AddItem_Should_ThrowException_IfItemWithSameIdExists()
        {
            await Assert.ThrowsAsync<SaveTodoItemException>(async () =>
                await _todoItemsRepository.AddItem(
                    new TodoItem() { Id = 2, Description = "a new item", IsCompleted = false }));
        }

        [Fact]
        public async Task AddItem_Should_ThrowException_IfItemWithSameDescriptionExists()
        {
            await Assert.ThrowsAsync<SaveTodoItemException>(async () =>
                await _todoItemsRepository.AddItem(
                    new TodoItem() { Id = 0, Description = "Test", IsCompleted = false }));
        }

        [Fact]
        public async Task AddItem_Should_TodoItem()
        {
            var item = new TodoItem() { Id = 0, Description = "a new item to be completed", IsCompleted = false };
            await _todoItemsRepository.AddItem(item);

            Assert.NotEqual(0, item.Id);
        }

        [Fact]
        public async Task UpdateItem_Should_ThrowException_IfItemDoesNotExist()
        {
            await Assert.ThrowsAsync<SaveTodoItemException>(async () =>
                await _todoItemsRepository.UpdateItem(5, 
                    new TodoItem() { Id = 5, Description = "Test", IsCompleted = false }));
        }
    }
}
