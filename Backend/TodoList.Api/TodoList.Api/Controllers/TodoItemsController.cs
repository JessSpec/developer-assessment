using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TodoList.Api.Entities;
using TodoList.Api.Repositories;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemsRepository _todoItemRepository;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(ITodoItemsRepository todoItemsRepository, ILogger<TodoItemsController> logger)
        {
            _todoItemRepository = todoItemsRepository;
            //todo: logger is unused
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            var results = await _todoItemRepository.FindAllItems();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(int id)
        {
            var result = await _todoItemRepository.FindItemById(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            //todo: not sure this check is needed?
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            await _todoItemRepository.UpdateItem(id, todoItem);

            return NoContent();
        } 

        [HttpPost]
        public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
        {
            if (string.IsNullOrEmpty(todoItem?.Description))
            {
                return BadRequest("Description is required");
            }
            else if (_todoItemRepository.TodoItemDescriptionExists(todoItem.Description))
            {
                return BadRequest("Description already exists");
            }

            //todo: error handling here.
            await _todoItemRepository.AddItem(todoItem);
             
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        } 
    }
}
