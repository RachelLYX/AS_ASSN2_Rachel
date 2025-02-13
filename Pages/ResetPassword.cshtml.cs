using AS_ASSN2_Rachel.Model;
using AS_ASSN2_Rachel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AS_ASSN2_Rachel.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ResetPasswordModel> _logger;
        private readonly IEmailSender _emailSender;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<ResetPasswordModel> logger, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string Message { get; set; } = "";

        public class InputModel
        {
            [Required]
            public string Token { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

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
            if (!ModelState.IsValid)
            {
                Message = "Invalid input.";
                return Page();
            }

            _logger.LogInformation($"Reset password attempt for {Input.Email}");

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                Message = "No account found with this email.";
                _logger.LogWarning($"Reset password failed: User with email {Input.Email} was not found.");
                return Page();
            }

            var token = Input.Token;

            var result = await _userManager.ResetPasswordAsync(user, token, Input.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning($"Password reset error for {Input.Email}: {error.Description}");

                    if (error.Code == "InvalidToken")
                    {
                        _logger.LogInformation($"Invalid token detected for {Input.Email}, generating a new one.");

                        var newToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                        user.SecurityStamp = Guid.NewGuid().ToString();
                        await _userManager.UpdateAsync(user);

                        await _userManager.UpdateSecurityStampAsync(user);

                        var resetUrl = Url.Page(
                            "/ResetPassword",
                            pageHandler: null,
                            values: new { token = newToken, email = Input.Email },
                            protocol: Request.Scheme
                            );

                        _logger.LogInformation($"Generated reset URL: {resetUrl}");

                        await _emailSender.SendEmailAsync(
                            Input.Email,
                            "Password Reset Request",
                            $"Your password reset token has expired. Please click the link to reset your password: {resetUrl}"
                            );
                        Message = "The reset token has expired. A new token has been generated and sent to your email.";
                        return Page();
                    }
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            _logger.LogInformation($"Password reset successful for {Input.Email}");
            await _signInManager.SignOutAsync();

            var signInResult = await _signInManager.PasswordSignInAsync(user, Input.NewPassword, false, false);

            if (signInResult.Succeeded)
            {
                Message = "Password reset successful, you are now logged in.";
                return RedirectToPage("/Index");
            }
            else
            {
                Message = "Error logging in with the new password. Please try again.";
                _logger.LogWarning($"Sign-in failed for {Input.Email} after reset.");
                return Page();
            }
        }
        public void OnGet(string token, string email)
        {
            _logger.LogInformation($"Raw Token: {token}");
            Input.Token = token;
            Input.Email = email;
        }
    }
}
