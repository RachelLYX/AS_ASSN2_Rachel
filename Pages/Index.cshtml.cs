using AS_ASSN2_Rachel.Helpers;
using AS_ASSN2_Rachel.Model;
using AS_ASSN2_Rachel.Services;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AS_ASSN2_Rachel.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SomeService _someService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string EmailAddress { get; set; }
        public string NRIC { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string WhoAmI { get; set; }

        public IndexModel(ILogger<IndexModel> logger, SomeService someService, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _someService = someService;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            // Sanitize session values
            FirstName = SanitizeInput(HttpContext.Session.GetString("FirstName"));
            LastName = SanitizeInput(HttpContext.Session.GetString("LastName"));
            EmailAddress = SanitizeInput(HttpContext.Session.GetString("EmailAddress"));
            Gender = SanitizeInput(HttpContext.Session.GetString("Gender"));

            // Retrieve encrypted NRIC and decryption configuration
            NRIC = SanitizeInput(HttpContext.Session.GetString("NRIC"));
           

            // Set other session values
            DateOfBirth = DateOfBirth;
            WhoAmI = SanitizeInput(HttpContext.Session.GetString("WhoAmI"));

            // Fetch user information from the database
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                FirstName = SanitizeInput(user.FirstName);
                LastName = SanitizeInput(user.LastName);
                EmailAddress = SanitizeInput(user.Email);
                Gender = SanitizeInput(user.Gender);

                NRIC = SanitizeInput(user.NRIC);
                // Set other user details
                DateOfBirth = DateOfBirth;
                WhoAmI = SanitizeInput(user.WhoAmI);
            }
        }

        // Sanitizes input by removing any potentially dangerous HTML
        private string SanitizeInput(string input)
        {
            var sanitizer = new HtmlSanitizer();
            return sanitizer.Sanitize(input);
        }
    }
}

