using AS_ASSN2_Rachel.Model;
using AS_ASSN2_Rachel.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AS_ASSN2_Rachel.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        private string EncryptNRIC(string nric)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
                aes.IV = Encoding.UTF8.GetBytes("1234567890123456");
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(nric);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var existingUser = await userManager.FindByEmailAsync(RModel.EmailAddress);
                if (existingUser != null)
                {
                    ModelState.AddModelError("RModel.EmailAddress", "Email address is already taken.");
                    return Page();
                }

                if (RModel.Resume != null && RModel.Resume.Length > 0)
                {
                    var allowedExtensions = new[] { ".docx", ".pdf" };
                    var fileExtension = Path.GetExtension(RModel.Resume.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("RModel.Resume", "Only .docx and .pdf files are allowed.");
                        return Page();
                    }

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, Path.GetFileName(RModel.Resume.FileName));

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await RModel.Resume.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    ModelState.AddModelError("RModel.Resume", "The Resume field is required.");
                    return Page();
                }

                var encryptedNRIC = EncryptNRIC(RModel.NRIC);

                var user = new ApplicationUser()
                {
                    FirstName = RModel.FirstName,
                    LastName = RModel.LastName,
                    Gender = RModel.Gender,
                    NRIC = encryptedNRIC,
                    Email = RModel.EmailAddress,
                    DateOfBirth = RModel.DateOfBirth,
                    ResumePath = Path.Combine("uploads", Path.GetFileName(RModel.Resume.FileName)),
                    WhoAmI = RModel.WhoAmI,
                    UserName = RModel.EmailAddress
                };
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);

                }
            }
            return Page();
        }
        public void OnGet()
        {
        }
    }
}
