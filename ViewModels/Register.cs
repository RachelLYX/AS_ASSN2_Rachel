using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace AS_ASSN2_Rachel.ViewModels
{
    public class Register
    {
        [Required]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string NRIC { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters.")]
    [RegularExpression(@"(?i)^(?=[a-z])(?=.*[0-9])([a-z0-9!@#$%\^&*()_?+\-=]){8,15}$", 
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Resume { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string WhoAmI { get; set; }

        public string? ReCaptcha { get; set; }
    }
}
