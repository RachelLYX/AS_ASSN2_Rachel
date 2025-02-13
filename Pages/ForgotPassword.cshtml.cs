using AS_ASSN2_Rachel.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using AS_ASSN2_Rachel.Services;

namespace AS_ASSN2_Rachel.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Message { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                Message = "If this email exists, a reset link has been sent.";
                return Page();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = Url.Page("/ResetPassword", null, new { token, email = user.Email }, Request.Scheme);

            await _emailSender.SendPasswordResetLinkAsync(user.Email, "Reset Password", resetUrl);

            Message = "Check your email for the reset link.";
            return Page();
        }
        public void OnGet()
        {
        }
    }
}
