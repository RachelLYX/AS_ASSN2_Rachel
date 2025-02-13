using System.ComponentModel.DataAnnotations;

namespace AS_ASSN2_Rachel.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string PasswordHash { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public string ResumePath { get; set; }

        [Required]
        public string WhoAmI { get; set; }
    }
}
