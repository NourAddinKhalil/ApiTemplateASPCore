using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITemplate.Data;
using APITemplate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using APITemplate.Helpers;
using APITemplate.Services;
using APITemplate.Constants;

namespace APITemplate.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: api/Task
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserTask>>> GetTask()
        {
            try
            {
                var tasks = await _taskService.GetTask();
                return Ok(
                    new
                {
                        tasks,
                });
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        // GET: api/Task/MyTasks
        [HttpGet("/MyTasks")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<UserTask>>> MyTasks()
        {
            try
            {
                var tasks = await _taskService.MyTasks();

                return Ok(new
                {
                    tasks
                });
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        // GET: api/Task/TaskFilterModel
        [HttpGet("FilterTasks")]
        public async Task<ActionResult<IEnumerable<UserTask>>> FilterTasks([FromQuery] TaskFilterModel filter)
        {
            try
            {
                var tasks = await _taskService.FilterTasks(filter);

                return Ok(new
                {
                    tasks,
                });
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        // GET: api/Task/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserTask>> GetTask(int id)
        {
            try
            {
                var task = await _taskService.GetTask(id);

                return Ok(new
                {
                    task,
                });
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        // PUT: api/Task/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTask(int id, UserTask task)
        {
            try
            {
                await _taskService.PutTask(id, task);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_taskService.TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }

            return NoContent();
        }

        // POST: api/Task
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserTask>> PostTask(UserTask task)
        {
            try
            {
                var id = await _taskService.PostTask(task);
                return CreatedAtAction("GetTask", new { id }, new { task });
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        // POST: api/Task/AssignTask/5
        [HttpPatch("AssignTask/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTask(int id, string userID)
        {
            try
            {
                //await _context.SaveChangesAsync();
                var task = await _taskService.AssignTask(id, userID);

                return Ok(new
                {
                    message = "Task Assigned Successfully!",
                    task
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_taskService.TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        // POST: api/Task/AssignTask/5
        [HttpPatch("ChangeMyTaskStatus/{id}")]
        public async Task<IActionResult> ChangeMyTaskStatus(int id, TaskStatuses status)
        {
            try
            {
                var task = await _taskService.ChangeMyTaskStatus(id, status);
                //await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Task status changed successfully!",
                    task
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_taskService.TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        // DELETE: api/Task/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await _taskService.DeleteTask(id);
            }
            catch (CustomAPIExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
            return NoContent();
        }
    }
}
