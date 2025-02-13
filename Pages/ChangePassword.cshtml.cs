using AS_ASSN2_Rachel.Model;
using AS_ASSN2_Rachel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AS_ASSN2_Rachel.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserService _userService;

        public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, UserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        [BindProperty]
        public ChangePasswordInputModel Input { get; set; }

        public class ChangePasswordInputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [MinLength(12, ErrorMessage = "Password must be at least 12 characters long.")]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("Login");
            }

            var result = await _userService.ChangeUserPasswordAsync(user, Input.CurrentPassword, Input.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["SuccessMessage"] = "Your password has been changed successfully.";
                return RedirectToPage("Index");
            }

            foreach (var error in result.Errors)
            {
                if (error.Code == "PasswordReuse")
                {
                    ModelState.AddModelError("Input.NewPassword", error.Description);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
        public void OnGet()
        {
        }
    }
}
