using AutoMapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Core.Entities;
using TodoList.Api.Models;
using TodoList.Core.Repositories;
using Xunit;
using TodoList.MinimalApi.Handlers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TodoList.Api.UnitTests
{
    public class TodoItemsHandlerTests
    {
        private ITodoItemsRepository _repositoryMock;
        private Mapper _mapper;
        private List<TodoItem> _inMemoryList;
        private const int nullId = 4;

        public TodoItemsHandlerTests()
        {
            _inMemoryList = new List<TodoItem>()
            {
                new TodoItem() {Id = 1, Description = "Refactor", IsCompleted = false },
                new TodoItem() {Id = 2, Description = "Test", IsCompleted = false },
            };

            _repositoryMock = Substitute.For<ITodoItemsRepository>();
            _repositoryMock.FindAllItems().Returns(_inMemoryList);
            _repositoryMock.FindItemById(1).Returns(_inMemoryList.Where(t => t.Id == 1).FirstOrDefault());
            _repositoryMock.FindItemById(nullId).ReturnsNull();

            var mapperConfiguration = new MapperConfiguration(
                config => config.AddProfile<Profiles.TodoItemProfile>());
            _mapper = new Mapper(mapperConfiguration);            
        }

        [Fact]
        public async Task Get_Should_ReturnItems()
        {
            var response = await TodoItemsHandlers.GetTodoItems(_repositoryMock, _mapper);

            Assert.IsType<Ok<IList<TodoItemDto>>>(response);
            var value = response.Value;

            Assert.NotNull(value);
            Assert.Equal(_inMemoryList.Count(), value.Count());

        }

        [Fact]
        public async Task GetById_Should_ReturnItem()
        {
            var response = await TodoItemsHandlers.GetTodoItem(_repositoryMock, _mapper, 1);
            
            var data = (Ok<TodoItemDto>)response.Result;
            Assert.NotNull(data);

            Assert.IsType<TodoItemDto>(data.Value);
            Assert.Equal(1, data.Value.Id);
        }

        [Fact]
        public async Task GetById_Should_ReturnNotFound()
        {
            var response = await TodoItemsHandlers.GetTodoItem(_repositoryMock, _mapper, nullId);

            var data = (NotFound)response.Result;
            Assert.NotNull(data);
        }

        [Fact]
        public async Task PutTodoItem_Should_UpdateExistingItem()
        {
            var testTodoItem = new TodoItemDto() { Id = 1, Description = "My updated description", IsCompleted = false };

            var response = await TodoItemsHandlers.PutTodoItem(_repositoryMock, _mapper, testTodoItem, 1);

            var data = (NoContent)response.Result;
            Assert.NotNull(data);
        }

        [Fact]
        public async Task PutTodoItem_ShouldNot_UpdateWhereIdsDoNotMatch()
        {
            var testTodoItem = new TodoItemDto() { Id = 1, Description = "My updated description", IsCompleted = true };
            var response = await TodoItemsHandlers.PutTodoItem(_repositoryMock, _mapper, testTodoItem, 4);

            var data = (BadRequest<string>)response.Result;
            Assert.NotNull(data);
        }


        [Fact]
        public async Task PostTodoItem_Should_CreateNewItem()
        {
            var testTodoItem = new TodoItemDto() { Id = 0, Description = "My new item", IsCompleted = false };
            var response = await TodoItemsHandlers.PostTodoItem(_repositoryMock, _mapper, testTodoItem);

            var data = (Created<TodoItemDto>)response.Result;
            Assert.NotNull(data);
        }

        [Fact]
        public async Task PostTodoItem_Should_ValidateDescription()
        {
            var testTodoItem = new TodoItemDto() { Id = 0, Description = string.Empty, IsCompleted = false };
            var response = await TodoItemsHandlers.PostTodoItem(_repositoryMock, _mapper, testTodoItem);

            var data = (BadRequest<string>)response.Result;
            Assert.NotNull(data);
        }
    }
}
