using APITemplate.Constants;
using APITemplate.DBModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APITemplate.Models
{
    [Table("Tasks")]
    public class UserTask
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Title { get; set; }
        public string Description { get; set; } = String.Empty;
        [Required]
        public TaskStatuses Status { get; set; } = TaskStatuses.NotAssigned;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed), DefaultValue("GETUTCDATE()")]
        public DateTime? CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed), DefaultValue("GETUTCDATE()")]
        public DateTime? UpdatedAt { get; set; }
        [MaxLength(450)]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}