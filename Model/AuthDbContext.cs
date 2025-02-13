using AS_ASSN2_Rachel.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AS_ASSN2_Rachel.Model
{
    public class AuthDbContext:IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;

        public AuthDbContext(DbContextOptions<AuthDbContext> options, IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AuditLog>().ToTable("AuditLogs");
        }
    }
}
