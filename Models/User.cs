using System.ComponentModel.DataAnnotations;

namespace Neemah.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must include uppercase, lowercase, and a number.")]
        public string Password { get; set; }

        [Required]
        public string UserType { get; set; }
    }
}
