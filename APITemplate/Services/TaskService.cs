using APITemplate.Constants;
using APITemplate.Data;
using APITemplate.Helpers;
using APITemplate.Models;
using Microsoft.EntityFrameworkCore;

namespace APITemplate.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public TaskService(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        public async Task<IEnumerable<UserTask>> GetTask()
        {
            if (_context.Tasks == null)
            {
                throw new CustomAPIExceptionHelper("Entity set 'ApplicationDbContext.Task'  is null.", StatusCodes.Status500InternalServerError);
            }

            var tasks = await _context.Tasks.ToListAsync();

            return tasks;
        }

        public async Task<IEnumerable<UserTask>> MyTasks()
        {
            if (_context.Tasks == null)
            {
                throw new CustomAPIExceptionHelper("Entity set 'ApplicationDbContext.Task'  is null.", StatusCodes.Status500InternalServerError);
            }
            var userID = _userService.CurrentUserID;

            if (string.IsNullOrEmpty(userID))
            {
                throw new CustomAPIExceptionHelper("Unauthorized To view this data.", StatusCodes.Status401Unauthorized);
            }

            var tasks = await _context.Tasks.Where(t => t.UserId == userID).ToListAsync();

            return tasks;
        }

        public async Task<IEnumerable<UserTask>> FilterTasks(TaskFilterModel filter)
        {
            if (_context.Tasks == null)
            {
                throw new CustomAPIExceptionHelper("Entity set 'ApplicationDbContext.Task'  is null.", StatusCodes.Status500InternalServerError);
            }

            if (filter == null)
            {
                throw new CustomAPIExceptionHelper("Must pass at least one field to filter by!.", StatusCodes.Status400BadRequest);
            }

            bool filtered = false;
            var results = _context.Tasks.Include(t => t.User).AsQueryable();


            if (!string.IsNullOrEmpty(filter.Title))
            {
                filtered = true;
                if (filter.ContainsAString)
                {
                    results = results.Where(t => t.Title.Contains(filter.Title));
                }
                else
                {
                    results = results.Where(t => t.Title == filter.Title);
                }
            }

            if (filter.Status != null)
            {
                filtered = true;
                results = results.Where(t => t.Status == filter.Status);
            }

            if (!string.IsNullOrEmpty(filter.UserName))
            {
                filtered = true;
                if (filter.ContainsAString)
                {
                    results = results.Where(t => t.User != null && t.User.FullName.Contains(filter.UserName ?? ""));
                }
                else
                {
                    results = results.Where(t => t.User != null && t.User.FullName == (filter.UserName ?? ""));
                }
            }

            if (!await _userService.IsAdmin())
            {
                results = results.Where(t => t.UserId == _userService.CurrentUserID);
            }

            if (!filtered)
            {
                throw new CustomAPIExceptionHelper("Must pass at least one field to filter by!.", StatusCodes.Status400BadRequest);
            }

            if (!results.Any())
            {
                throw new CustomAPIExceptionHelper("No Data Found!.", StatusCodes.Status404NotFound);
            }

            var tasks = await results.ToListAsync();

            return tasks;
        }

        public async Task<UserTask> GetTask(int id)
        {
            if (_context.Tasks == null)
            {
                throw new CustomAPIExceptionHelper("Entity set 'ApplicationDbContext.Task'  is null.", StatusCodes.Status500InternalServerError);
            }

            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new CustomAPIExceptionHelper("No Data Found!.", StatusCodes.Status404NotFound);
            }

            if (!_userService.CheckIfModelBelongToCurrentUser(task.UserId ?? "") && !await _userService.IsAdmin())
            {
                throw new CustomAPIExceptionHelper("You are not allowed to view this task.", StatusCodes.Status401Unauthorized);
            }

            return task;
        }

        public async Task PutTask(int id, UserTask task)
        {
            if (_context.Tasks == null)
            {
                throw new CustomAPIExceptionHelper("Entity set 'ApplicationDbContext.Task'  is null.", StatusCodes.Status500InternalServerError);
            }

            if (id != task.Id)
            {
                throw new CustomAPIExceptionHelper("ID Not Found!.", StatusCodes.Status400BadRequest);
            }

            _context.Entry(task).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task<int> PostTask(UserTask task)
        {
            if (_context.Tasks == null)
            {
                throw new CustomAPIExceptionHelper("Entity set 'ApplicationDbContext.Task'  is null.", StatusCodes.Status500InternalServerError);
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return task.Id;
        }

        public async Task<UserTask> AssignTask(int id, string userID)
        {
            if (id <= 0)
            {
                throw new CustomAPIExceptionHelper("Must specify Task ID!.", StatusCodes.Status400BadRequest);
            }

            if (string.IsNullOrEmpty(userID))
            {
                throw new CustomAPIExceptionHelper("Must specify User!.", StatusCodes.Status400BadRequest);
            }

            if (_context.Tasks == null)
            {
                throw new CustomAPIExceptionHelper("Entity set 'ApplicationDbContext.Task'  is null.", StatusCodes.Status500InternalServerError);
            }

            var task = _context.Tasks.SingleOrDefault(task => task.Id == id);

            if (task == null)
            {
                throw new CustomAPIExceptionHelper("Task not found!.", StatusCodes.Status404NotFound);
            }

            if (task.Status != TaskStatuses.NotAssigned)
            {
                throw new CustomAPIExceptionHelper("Task already assigned!.", StatusCodes.Status400BadRequest);
            }

            task.Status = TaskStatuses.Assigned;
            task.UserId = userID;

            _context.Entry(task).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            task.User = _userService.GetUserByID(userID);
            return task;
        }

        public async Task<UserTask> ChangeMyTaskStatus(int id, TaskStatuses status)
        {
            if (id <= 0)
            {
                throw new CustomAPIExceptionHelper("Must specify Task ID!.", StatusCodes.Status400BadRequest);
            }

            if (status == TaskStatuses.NotAssigned || status == TaskStatuses.Assigned)
            {
                throw new CustomAPIExceptionHelper("Must Specify the task status in 'InProgress', 'Completed'!.", StatusCodes.Status400BadRequest);
            }

            if (_context.Tasks == null)
            {
                throw new CustomAPIExceptionHelper("Entity set 'ApplicationDbContext.Task'  is null.", StatusCodes.Status500InternalServerError);
            }

            var task = _context.Tasks.SingleOrDefault(task => task.Id == id);

            if (task == null)
            {
                throw new CustomAPIExceptionHelper("Task not found!.", StatusCodes.Status404NotFound);
            }

            if (task.Status == TaskStatuses.Completed && !await _userService.IsAdmin())
            {
                throw new CustomAPIExceptionHelper("Once the task stauts is 'Completed' only admin can change the task status!.", StatusCodes.Status400BadRequest);
            }

            if (task.Status == status)
            {
                throw new CustomAPIExceptionHelper($"Task already {status}!.", StatusCodes.Status400BadRequest);
            }

            if (!_userService.CheckIfModelBelongToCurrentUser(task.UserId ?? "") && !await _userService.IsAdmin())
            {
                throw new CustomAPIExceptionHelper("You are not allowed to edit this task!.", StatusCodes.Status401Unauthorized);
            }

            task.Status = status;

            _context.Entry(task).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return task;
        }

        public async Task DeleteTask(int id)
        {
            if (_context.Tasks == null)
            {
                throw new CustomAPIExceptionHelper("Entity set 'ApplicationDbContext.Task'  is null.", StatusCodes.Status500InternalServerError);
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                throw new CustomAPIExceptionHelper("Task not found!.", StatusCodes.Status404NotFound);
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public bool TaskExists(int id)
        {
            return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
