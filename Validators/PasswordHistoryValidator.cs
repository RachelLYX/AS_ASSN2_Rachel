using AS_ASSN2_Rachel.Model;
using Microsoft.AspNetCore.Identity;

namespace AS_ASSN2_Rachel.Validators
{
    public class PasswordHistoryValidator : IPasswordValidator<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int _maxPasswordHistory = 2;

        public PasswordHistoryValidator()
        {
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
        {
            var passwordHistory = await GetPasswordHistoryAsync(user);

            if (passwordHistory.Contains(password))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "PasswordReuse",
                    Description = "You cannot reuse your last 2 passwords."
                });
            }

            return IdentityResult.Success;
        }

        private Task<string[]> GetPasswordHistoryAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PasswordHistory?.Take(_maxPasswordHistory).ToArray());
        }
    }
}
