using APITemplate.Constants;

namespace APITemplate.Models
{
    public class TaskFilterModel
    {
        public string? Title { get; set; }
        public TaskStatuses? Status { get; set; }
        public string? UserName { get; set; }
        public bool ContainsAString { get; set; } = false;
    }
}
