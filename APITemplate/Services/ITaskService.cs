using APITemplate.Constants;
using APITemplate.Models;

namespace APITemplate.Services
{
    public interface ITaskService
    {
        Task<UserTask> AssignTask(int id, string userID);
        Task<UserTask> ChangeMyTaskStatus(int id, TaskStatuses status);
        Task DeleteTask(int id);
        Task<IEnumerable<UserTask>> FilterTasks(TaskFilterModel filter);
        Task<IEnumerable<UserTask>> GetTask();
        Task<UserTask> GetTask(int id);
        Task<IEnumerable<UserTask>> MyTasks();
        Task<int> PostTask(UserTask task);
        Task PutTask(int id, UserTask task);
        bool TaskExists(int id);
    }
}