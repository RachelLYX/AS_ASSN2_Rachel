using AS_ASSN2_Rachel.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AS_ASSN2_Rachel.Pages
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            if (userId == null || token == null)
            {
                Message = "Invalid confirmation link.";
                return Page();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Message = "User not found.";
                return Page();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                Message = "Your email has been confirmed. You can now log in.";
            }
            else
            {
                Message = "Email confirmation failed.";
            }
            return Page();
        }
    }
}
