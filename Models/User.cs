using System.ComponentModel.DataAnnotations;

namespace ClinicWebsite.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; } = "User"; // default role
    }
}
