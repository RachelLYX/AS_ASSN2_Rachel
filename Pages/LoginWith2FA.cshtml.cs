using AS_ASSN2_Rachel.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AS_ASSN2_Rachel.Pages
{
    public class LoginWith2FAModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginWith2FAModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public class InputModel
        {
            [Required]
            [Display(Name = "Authenticator Code")]
            public string TwoFactorCode { get; set; }

            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            Input = new InputModel { RememberMe = rememberMe };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl ??= Url.Content("~/");

            var result = await _signInManager.TwoFactorSignInAsync("Authenticator", Input.TwoFactorCode, rememberMe, false);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked. Try again later.");
                return Page();
            }
            ModelState.AddModelError(string.Empty, "Invalid authenticator code");
            return Page();
        }
    }
}
