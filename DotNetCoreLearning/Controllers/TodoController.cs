using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreLearning;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private static List<ToDo> _todos = new List<ToDo>
        {
            new ToDo { Id = 1, Title = "Day 1", IsCompleted = true },
            new ToDo { Id = 2, Title = "Day 2", IsCompleted = false }
        };

        [HttpGet]
        public ActionResult<IEnumerable<ToDo>> Get()
        {
            return Ok(_todos);
        }

        [HttpGet("{id}")]
        public ActionResult<ToDo> GetById(int id)
        {
            var todo = _todos.Find(t => t.Id == id);
            if (todo == null)
            {
                return NotFound($"Todo with ID {id} not found");
            }
            return Ok(todo);
        }

        [HttpPost]
        public ActionResult<ToDo> Create(ToDo todo)
        {
            if (string.IsNullOrEmpty(todo.Title))
            {
                return BadRequest("Title is required");
            }
            todo.Id = _todos.Count + 1;
            _todos.Add(todo);
            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ToDo updatedTodo)
        {
            var todo = _todos.Find(t => t.Id == id);
            if (todo == null)
            {
                return NotFound($"Todo with ID {id} not found");
            }
            if (string.IsNullOrEmpty(updatedTodo.Title))
            {
                return BadRequest("Title is required");
            }
            todo.Title = updatedTodo.Title;
            todo.IsCompleted = updatedTodo.IsCompleted;
            return Ok(new { Message = "Update successful" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var todo = _todos.Find(t => t.Id == id);
            if (todo == null)
            {
                return NotFound($"Todo with ID {id} not found");
            }

            _todos.Remove(todo);
            return Ok(new { Message = "Deletion successful" });
        }
    }
}
