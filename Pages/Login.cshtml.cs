using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AS_ASSN2_Rachel.ViewModels;
using Microsoft.AspNetCore.Identity;
using AS_ASSN2_Rachel.Model;
using AS_ASSN2_Rachel.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using AS_ASSN2_Rachel.Services;

namespace AS_ASSN2_Rachel.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserService _userService;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, UserService userService)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._userService = userService;
        }

        public string Message { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                Message = "Invalid username or password.";
                return Page();
            }
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("/LoginWith2FA", new { ReturnUrl = "/Index", Input.RememberMe });
            }

            if (result.Succeeded)
            {
                if (await _userManager.GetTwoFactorEnabledAsync(user))
                {
                    return RedirectToPage("/LoginWith2FA", new { ReturnUrl = "/Index", Input.RememberMe });
                }
                var userSessions = await GetUserSessionsAsync(user);

                if (userSessions.Count > 1)
                {
                    Message = "You are already from another device or tab.";
                    ModelState.AddModelError("", Message);
                    return Page();
                }
                await _userManager.ResetAccessFailedCountAsync(user);
                await _userManager.SetLockoutEndDateAsync(user, null);

                HttpContext.Session.SetString("SessionId", Guid.NewGuid().ToString());

                return RedirectToPage("/Index");
            }

            if (result.IsLockedOut)
            {
                Message = "Your account has been locked due to multiple failed login attempts. Please try again later.";
                ModelState.AddModelError("", Message);
                return Page();
            }
            Message = "Invalid username or password.";
            ModelState.AddModelError("", Message);
            return Page();
        }

        private async Task<List<string>> GetUserSessionsAsync(ApplicationUser user)
        {
            return new List<string>();
        }
    }
}
