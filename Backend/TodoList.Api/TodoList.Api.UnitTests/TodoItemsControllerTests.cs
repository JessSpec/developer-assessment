using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Controllers;
using TodoList.Api.Entities;
using TodoList.Api.Repositories;
using Xunit;

namespace TodoList.Api.UnitTests
{
    public class TodoItemsControllerTest
    {
        private readonly TodoItemsController _controller;
        private List<TodoItem> _inMemoryList;
        private const string existsDescription = "exists";
        private const int nullId = 4;

        public TodoItemsControllerTest()
        {
            _inMemoryList = new List<TodoItem>()
            {
                new TodoItem() {Id = 1, Description = "Refactor", IsCompleted = false },
                new TodoItem() {Id = 2, Description = "Test", IsCompleted = false },
            };

            var repositoryMock = Substitute.For<ITodoItemsRepository>();
            repositoryMock.FindAllItems().Returns(_inMemoryList);
            repositoryMock.FindItemById(1).Returns(_inMemoryList.Where(t => t.Id == 1).FirstOrDefault());
            repositoryMock.FindItemById(nullId).ReturnsNull();
            repositoryMock.TodoItemDescriptionExists(existsDescription).Returns(true);

            _controller = new TodoItemsController(repositoryMock);
        }

        [Fact]
        public async Task Get_Should_ReturnItems()
        {
            var response = await _controller.GetTodoItems();

            //todo: find a simpler way to do this.
            var viewResult = Assert.IsAssignableFrom<ActionResult>(response);
            var okResult = viewResult as OkObjectResult;

            Assert.NotNull(okResult);

            var data = okResult.Value as List<TodoItem>;
            Assert.NotNull(data);

            Assert.Equal(_inMemoryList.Count(), data.Count());
        }

        [Fact]
        public async Task GetById_Should_ReturnItem()
        {
            var response = await _controller.GetTodoItem(1);

            var viewResult = Assert.IsAssignableFrom<ActionResult>(response);
            var okResult = viewResult as OkObjectResult;

            Assert.NotNull(okResult);

            var data = okResult.Value as TodoItem;
            Assert.NotNull(data);
            Assert.Equal(1, data.Id);
        }

        [Fact]
        public async Task GetById_Should_ReturnNotFound()
        {
            var response = await _controller.GetTodoItem(nullId);

            var viewResult = Assert.IsAssignableFrom<ActionResult>(response);
            var notFound = viewResult as NotFoundResult;

            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task PutTodoItem_Should_UpdateExistingItem()
        {
            var testTodoItem = new TodoItem() { Id = 1, Description = "My updated description", IsCompleted = false };
            var response = await _controller.PutTodoItem(1, testTodoItem);

            var viewResult = Assert.IsAssignableFrom<ActionResult>(response);
            var noContent = viewResult as NoContentResult;

            Assert.NotNull(noContent);
        }

        [Fact]
        public async Task PutTodoItem_ShouldNot_UpdateNonExistingItem()
        {
            var testTodoItem = new TodoItem() { Id = 1, Description = "My updated description", IsCompleted = true };
            var response = await _controller.PutTodoItem(4, testTodoItem);

            var viewResult = Assert.IsAssignableFrom<ActionResult>(response);
            var noContent = viewResult as BadRequestResult;

            Assert.NotNull(noContent);
        }


        [Fact]
        public async Task PostTodoItem_Should_CreateNewItem()
        {
            var testTodoItem = new TodoItem() { Id = 0, Description = "My new item", IsCompleted = false };
            var response = await _controller.PostTodoItem(testTodoItem);

            var viewResult = Assert.IsAssignableFrom<ActionResult>(response);
            var createdAtAction = viewResult as CreatedAtActionResult;

            Assert.NotNull(createdAtAction);
        }

        [Fact]
        public async Task PostTodoItem_Should_ValidateDescription()
        {
            var testTodoItem = new TodoItem() { Id = 0, Description = string.Empty, IsCompleted = false };
            var response = await _controller.PostTodoItem(testTodoItem);

            var viewResult = Assert.IsAssignableFrom<ActionResult>(response);
            var badRequest = viewResult as BadRequestObjectResult;

            Assert.NotNull(badRequest);
        }


        [Fact]
        public async Task PostTodoItem_Should_ValidateDescriptionAlreadyExists()
        {
            var testTodoItem = new TodoItem() { Id = 0, Description = existsDescription, IsCompleted = false };
            var response = await _controller.PostTodoItem(testTodoItem);

            var viewResult = Assert.IsAssignableFrom<ActionResult>(response);
            var badRequest = viewResult as BadRequestObjectResult;

            Assert.NotNull(badRequest);
        }
    }
}
