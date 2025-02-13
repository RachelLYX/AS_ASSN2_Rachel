using AS_ASSN2_Rachel.Model;
using Microsoft.AspNetCore.Identity;

namespace AS_ASSN2_Rachel.Services
{
    public class UserManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int _maxPasswordHistory = 2;

        public UserManagerService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> SetPasswordAsync(ApplicationUser user, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, user.PasswordHash, newPassword);

            if (result.Succeeded)
            {
                if (user.PasswordHistory.Length >= _maxPasswordHistory)
                {
                    user.PasswordHistory = user.PasswordHistory.Skip(1).ToArray();
                }

                user.PasswordHistory = user.PasswordHistory.Append(newPassword).ToArray();

                await _userManager.UpdateAsync(user);
            }
            return result;
        }
    }
}
