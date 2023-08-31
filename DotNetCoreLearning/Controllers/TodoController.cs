using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreLearning.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DotNetCoreLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoContext _context;

        public ToDoController(ToDoContext context)
        {
            _context = context;
        }

        // GET: api/ToDoes
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer",Roles ="User,Admin")]       
        public async Task<ActionResult<IEnumerable<ToDoDto>>> GetTodoItems()
        {
            if (_context.TodoItems == null)
            {
                return NotFound("No records found");
            }
            List<ToDoDto> toDoDtoList = new List<ToDoDto>();
            List<ToDo> toDoList = new List<ToDo>();

            if (!User.IsInRole(UserRoles.Admin))
            {
                toDoList = _context.TodoItems
                 .Where(record => record.CreatedBy == User.Identity.Name)
                 .ToList();
            }
            else
            {
                toDoList = await _context.TodoItems.ToListAsync();
            }

            foreach (var item in toDoList)
            {
                var dtoItem = new ToDoDto();

                dtoItem.Title = item.Title;
                dtoItem.IsCompleted = item.IsCompleted;

                toDoDtoList.Add(dtoItem);
            }
            return toDoDtoList;
        }

        // GET: api/ToDoes/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer",Roles = "User,Admin")]       
        public async Task<ActionResult<ToDoDto>> GetToDo(int id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound("No records found");
            }
            ToDoDto toDoDtoItem = new ToDoDto();
            ToDo toDoItem = new ToDo();

         
            if (!User.IsInRole(UserRoles.Admin))
            {
                toDoItem = await _context.TodoItems
                 .Where(record => record.Id==id && record.CreatedBy == User.Identity.Name)
                 .FirstOrDefaultAsync();
            }
            else
            {
                toDoItem = await _context.TodoItems.FindAsync(id);
            }
            if (toDoItem == null)
            {
                return NotFound($"Todo with ID {id} not found");
            }

            toDoDtoItem.Title = toDoItem.Title;
            toDoDtoItem.IsCompleted = toDoItem.IsCompleted;

            return toDoDtoItem;
        }
       
        //// PUT: api/ToDoes/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User,Admin")]
        public async Task<IActionResult> PutToDo(int id, ToDoDto toDo)
        {             
            var itemToDoExists = ToDoExists(id);
            if (!itemToDoExists)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    var itemToUpdate = _context.TodoItems.FirstOrDefault(item => item.Id == id);

                    if (itemToUpdate == null)
                    {
                        return NotFound();
                    }

                    itemToUpdate.Title = toDo.Title;
                    itemToUpdate.IsCompleted = toDo.IsCompleted;

                    _context.Entry(itemToUpdate).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return Ok(new { Message = "Update successful" });           
        }

        // POST: api/ToDoes       
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User,Admin")]
        public async Task<ActionResult<ToDoDto>> PostToDo(ToDoDto toDo)
        {
            if (_context.TodoItems == null)
            {
                return Problem("Entity set 'ToDoContext.TodoItems'  is null.");
            }
            var toDoModel = new ToDo
            {
                Title = toDo.Title,
                IsCompleted = toDo.IsCompleted,
                CreatedBy = User.Identity.Name
            };
            
            _context.TodoItems.Add(toDoModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostToDo", toDoModel);
        }

        // DELETE: api/ToDoes/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User,Admin")]
        public async Task<IActionResult> DeleteToDo(int id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            ToDo item = new ToDo();
          
            if (!User.IsInRole(UserRoles.Admin))
            {
                item = await _context.TodoItems
                            .Where(item => item.Id == id && item.CreatedBy == User.Identity.Name) // Add the where clause
                            .FirstOrDefaultAsync();
            }
            else
            {
               item = await _context.TodoItems.FindAsync(id);
            }
          
            if (item == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Deletion successful" });            
        }

        private bool ToDoExists(int id)
        {
            if (!User.IsInRole(UserRoles.Admin))
                return (_context.TodoItems?.Any(e => e.Id == id && e.CreatedBy == User.Identity.Name)).GetValueOrDefault();
            else
                return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
