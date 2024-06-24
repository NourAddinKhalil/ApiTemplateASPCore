using System.ComponentModel.DataAnnotations;

namespace APITemplate.Models
{
    public class RegisterModel
    {
        [Required, StringLength(100)]
        public string FullName { get; set; }
        [Required, StringLength(256)]
        public string UserName { get; set; }
        [Required, StringLength(256)]
        public string Email { get; set; }
        [Required, StringLength(40, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
