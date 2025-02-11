﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoList.Core.Contexts;
using TodoList.Core.Entities;
using TodoList.Core.Exceptions;

namespace TodoList.Core.Repositories
{
    public class TodoItemsRepository : ITodoItemsRepository
    {
        private readonly TodoContext _context;
        private readonly ILogger<ITodoItemsRepository> _logger;
        public TodoItemsRepository(TodoContext context, ILogger<ITodoItemsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IList<TodoItem>> FindAllItems()
        {
            return await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();
        }

        public async Task<TodoItem> FindItemById(int id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task UpdateItem(int id, TodoItem todoItem)
        {
            //db validations
            //todo: find a way to combine these two validations to avoid two round trips to db            
            if (!await TodoItemIdExists(todoItem.Id))
            {
                var message = $"Cannot update. Todo Item does not exist";
                _logger.LogError(message);
                throw new SaveTodoItemException(message);
            }

            //todo: still issue here - if update iscompleted only then this validation fails!
            //if (await TodoItemDescriptionExists(todoItem.Description))
            //{
            //    var message = $"Cannot update. Todo Item with description {todoItem.Description} already exists";
            //    _logger.LogError(message);
            //    throw new SaveTodoItemException(message);
            //}

            _context.Update(todoItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)  //todo: collapse these to general Exception as all catch blocks are the same
            {
                _logger.LogError(ex.Message);

                throw new SaveTodoItemException(ex.Message);              
            }
            catch(OperationCanceledException opEx)
            {
                _logger.LogError(opEx.Message);
                throw new SaveTodoItemException(opEx.Message);
            }
            
        }

        public async Task AddItem(TodoItem todoItem)
        {
            //db validations
            if (await TodoItemExists(todoItem))
            {
                var message = $"Cannot Create. Todo Item {todoItem.Description} already exists";
                _logger.LogError(message);
                throw new SaveTodoItemException(message);
            }

            _context.TodoItems.Add(todoItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)  //todo: collapse these to general Exception as all catch blocks are the same
            {
                _logger.LogError(ex.Message);
                throw new SaveTodoItemException(ex.Message);
            }
            catch (OperationCanceledException opEx)
            {
                _logger.LogError(opEx.Message);
                throw new SaveTodoItemException(opEx.Message);
            }
            catch(Exception e) 
            {
                _logger.LogError(e.Message);
                throw new SaveTodoItemException(e.Message);
            }
        }

        private async Task<bool> TodoItemExists(TodoItem todoItem)
        {
            return await _context.TodoItems
                .AnyAsync(x => x.Id == todoItem.Id || x.Description == todoItem.Description);
        }

        private async Task<bool> TodoItemIdExists(int id)
        {
            return await _context.TodoItems.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> TodoItemDescriptionExists(string description)
        {
            return await _context.TodoItems
                   .AnyAsync(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
        }
    }
}
