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
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using AS_ASSN2_Rachel.Services;

namespace AS_ASSN2_Rachel.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        private readonly IEmailSender _emailSender;
        private const string ReCaptchaSecretKey = "6LdJ584qAAAAAEMpRaUe263YYbO8oQeAfRuCS9zh";

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _emailSender = emailSender;
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

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var recaptchaResponse = Request.Form["g-recaptcha-response"];

            if (string.IsNullOrEmpty(recaptchaResponse) || !await VerifyRecaptchaAsync(recaptchaResponse))
            {
                ModelState.AddModelError("Recaptcha", "reCAPTCHA verification failed. Please try again.");
            }

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
                    TempData["FirstName"] = RModel.FirstName;
                    TempData["LastName"] = RModel.LastName;
                    TempData["Gender"] = RModel.Gender;
                    TempData["EmailAddress"] = RModel.EmailAddress;
                    TempData["NRIC"] = RModel.NRIC;
                    TempData["DateOfBirth"] = RModel.DateOfBirth.ToDateTime(new TimeOnly(0, 0));
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);

                }
            return RedirectToPage("/Index");
        }

        private async Task<bool> VerifyRecaptchaAsync(string recaptchaResponse)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "secret", ReCaptchaSecretKey },
                    { "response", recaptchaResponse }
                };
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
                return jsonResponse.success == true;
            }
        }
    }
}
