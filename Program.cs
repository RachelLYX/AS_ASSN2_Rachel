using AS_ASSN2_Rachel.Model;
using AS_ASSN2_Rachel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using AS_ASSN2_Rachel.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<LoggingService>();
builder.Services.AddScoped<SomeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AS_ASSN2_Rachel.Services.IEmailSender, AS_ASSN2_Rachel.Services.EmailSender>();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnectionString")));

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;

    options.Password.RequiredLength = 12;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 5;

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = false;

})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders()
.AddPasswordValidator<PasswordHistoryValidator>();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(2);
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.LoginPath = "/Login";
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self'; img-src 'self' data:;");
    await next();
});

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseSession();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
