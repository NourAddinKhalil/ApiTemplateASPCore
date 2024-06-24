using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace APITemplate.DBModels
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; }
        public string? Image { get; set; }
    }
}
