using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS_ASSN2_Rachel.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? SessionId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string NRIC { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string WhoAmI { get; set; }

        public string ResumePath { get; set; }

        [NotMapped]
        public IFormFile Resume { get; set; }

        public string[] PasswordHistory { get; set; } = new string[0];

        public DateTime? LastPasswordChangeDate { get; set; }
    }
}
