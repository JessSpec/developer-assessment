using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Core.Entities;
using TodoList.Api.Models;
using TodoList.Core.Repositories;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemsRepository _todoItemRepository;
        private readonly IMapper _mapper;

        public TodoItemsController(ITodoItemsRepository todoItemsRepository, IMapper mapper)
        {
            _todoItemRepository = todoItemsRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            var results = await _todoItemRepository.FindAllItems();            
            return Ok(_mapper.Map<IList<TodoItem>, IList<TodoItemDto>>(results));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(int id)
        {
            var result = await _todoItemRepository.FindItemById(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TodoItem, TodoItemDto>(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItemDto todoItemDto)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            var todoItem = _mapper.Map<TodoItemDto, TodoItem>(todoItemDto);
            await _todoItemRepository.UpdateItem(id, todoItem);

            return NoContent();
        } 

        [HttpPost]
        public async Task<IActionResult> PostTodoItem(TodoItemDto todoItemDto)
        {
            if (string.IsNullOrEmpty(todoItemDto?.Description))
            {
                return BadRequest("Description is required");
            }
            else if (await _todoItemRepository.TodoItemDescriptionExists(todoItemDto.Description))
            {
                return BadRequest("Description already exists");
            }

            //todo: error handling here.
            var todoItem = _mapper.Map<TodoItemDto, TodoItem>(todoItemDto);
            await _todoItemRepository.AddItem(todoItem);
             
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        } 
    }
}
