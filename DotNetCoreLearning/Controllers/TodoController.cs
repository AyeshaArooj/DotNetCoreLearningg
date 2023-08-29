using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        // [Authorize(AuthenticationSchemes = "Bearer",Roles ="Admin")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserPolicy")] // Requires "User" role
        public async Task<ActionResult<IEnumerable<ToDo>>> GetTodoItems()
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            var usernameClaim = User.FindFirstValue(ClaimTypes.Name);

            if (usernameClaim != UserRoles.Admin)
            {
                var recordsCreatedByUser = _context.TodoItems
                 .Where(record => record.CreatedBy == usernameClaim)
                 .ToList();

                return recordsCreatedByUser;
            }

            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/ToDoes/5
        [HttpGet("{id}")]
        // [Authorize(AuthenticationSchemes = "Bearer",Roles = "User,Admin")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserPolicy")] // Requires "User" role
        public async Task<ActionResult<ToDo>> GetToDo(int id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound("No records found");
            }
            var toDo = await _context.TodoItems.FindAsync(id);

            if (toDo == null)
            {
                return NotFound($"Todo with ID {id} not found");
            }
            
            return toDo;
        }

        // PUT: api/ToDoes/5

        [HttpPut("{id}")]
        // [Authorize(AuthenticationSchemes = "Bearer",Roles = "User,Admin")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserPolicy")] // Requires "User" role
        public async Task<IActionResult> PutToDo(int id, ToDo toDo)
        {
            if (id != toDo.Id)
            {
                return BadRequest();
            }
           // toDo.CreatedBy = User.FindFirstValue(ClaimTypes.Name);
            _context.Entry(toDo).State = EntityState.Modified;
          
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ToDoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        // [Authorize(AuthenticationSchemes = "Bearer",Roles = "User,Admin")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "UserPolicy")] // Requires "User" role
        public async Task<ActionResult<ToDo>> PostToDo(ToDo toDo)
        {
            if (_context.TodoItems == null)
            {
                return Problem("Entity set 'ToDoContext.TodoItems'  is null.");
            }
           
            toDo.CreatedBy= User.FindFirstValue(ClaimTypes.Name);
            _context.TodoItems.Add(toDo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetToDo", new { id = toDo.Id }, toDo);
        }

        // DELETE: api/ToDoes/5
        [HttpDelete("{id}")]
        // [Authorize(AuthenticationSchemes = "Bearer",Roles = "Admin")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")] // Requires "User" role
        public async Task<IActionResult> DeleteToDo(int id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            var toDo = await _context.TodoItems.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(toDo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ToDoExists(int id)
        {
            return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
