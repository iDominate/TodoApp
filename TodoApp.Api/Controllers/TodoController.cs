using System.Collections.Immutable;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Common;
using TodoApp.Api.Requests;
using TodoApp.Application.TodoCQRS.Commands;
using TodoApp.Application.TodoCQRS.Queries;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Api.Controllers
{


    [Authorize]
    [ApiController]
    [Route("/api/v1/todos")]
    public class TodoController(ISender _sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllTodos()
        {
            var query = new GetAllTodosQuery();
            var result = await _sender.Send(query);
            return Ok(new TodoResponse<IImmutableList<Todo>>(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo(Guid id)
        {
            var query = new GetTodoByIdQuery(id.ToString());
            var result = await _sender.Send(query);
            if (result.IsError is true)
            {
                if (result.FirstError.Type is ErrorType.Validation)
                {
                    return BadRequest(new TodoResponse<IEnumerable<string>>(result.Errors.ConvertAll(e => e.Description), status: ResponseStatus.Error));
                }
                else
                {
                    return NotFound(new TodoResponse<string>(result.FirstError.Description));
                }
            }
            return Ok(new TodoResponse<Todo>(result.Value));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo(CreateTodoRequest request)
        {
            var command = request.AsTodoCommand();
            var result = await _sender.Send(command);
            if (result.IsError is true)
            {
                return BadRequest(new TodoResponse<string>(result.FirstError.Description, status: ResponseStatus.Error));
            }
            return Ok(new TodoResponse<Todo>(result.Value));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTodo(UpdateTodoRequest request)
        {
            var command = request.AsTodoCommand();
            var result = await _sender.Send(command);

            if (result.IsError is true)
            {
                return BadRequest(new TodoResponse<string>(result.FirstError.Description, status: ResponseStatus.Error));
            }
            return Ok(new TodoResponse<Todo>(result.Value));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(string id)
        {
            var command = new DeleteTodoCommand(id);
            var result = await _sender.Send(command);
            if (result.IsError is true)
            {
                return BadRequest(new TodoResponse<string>(result.FirstError.Description, status: ResponseStatus.Error));
            }
            return NoContent();
        }
    }
}