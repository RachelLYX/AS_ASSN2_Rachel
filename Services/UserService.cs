using AS_ASSN2_Rachel.Model;
using AS_ASSN2_Rachel.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AS_ASSN2_Rachel.Services
{
    public class UserService
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int _maxPasswordHistory = 2;

        public UserService(AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> ChangeUserPasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            user.PasswordHistory ??= new string[0];

            DateTime now = DateTime.Now;

            if (!await _userManager.CheckPasswordAsync(user, currentPassword))
            {
                Console.WriteLine("Password change failed. Invalid current password.");
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidCurrentPassword",
                    Description = "The current password is incorrect."
                });
            }

            if (user.PasswordHistory.Contains(newPassword))
            {
                Console.WriteLine($"Password change failed: Password reuse detected. New password matches one of the last {_maxPasswordHistory} passwords.");
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "Password Reuse",
                    Description = $"You cannot reuse any of your last {_maxPasswordHistory} passwords."
                });
            }

            foreach (var validator in _userManager.PasswordValidators)
            {
                var validationResult = await validator.ValidateAsync(_userManager, user, newPassword);
                if (!validationResult.Succeeded)
                {
                    Console.WriteLine("Password change failed: Validation errors occurred.");
                    foreach (var error in validationResult.Errors)
                    {
                        Console.WriteLine($"{error.Code}: {error.Description}");
                    }
                    return IdentityResult.Failed(validationResult.Errors.ToArray());
                }
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                var updatedHistory = user.PasswordHistory.ToList();
                if (updatedHistory.Count >= _maxPasswordHistory)
                {
                    updatedHistory.RemoveAt(0);
                }
                updatedHistory.Add(newPassword);
                user.PasswordHistory = updatedHistory.ToArray();

                user.LastPasswordChangeDate = now;

                _context.Entry(user).Property(u => u.LastPasswordChangeDate).IsModified = true;

                await _context.SaveChangesAsync();

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    Console.WriteLine("Failed to update user in the database.");
                    foreach (var error in updateResult.Errors)
                    {
                        Console.WriteLine($"{error.Code}: {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Password change failed. Errors:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"{error.Code}: {error.Description}");
                }
            }
            return result;
        }
    }
}

