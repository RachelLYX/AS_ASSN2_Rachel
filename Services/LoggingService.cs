using AS_ASSN2_Rachel.Model;
using AS_ASSN2_Rachel.ViewModels;

namespace AS_ASSN2_Rachel.Services
{
    public class LoggingService
    {
        private readonly AuthDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggingService(AuthDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string userId, string activity)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst("sub")?.Value ?? "Anonymous";
            var userName = _httpContextAccessor.HttpContext.User?.Identity?.Name ?? "Anonymous";
            var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            var log = new AuditLog
            {
                UserId = userId,
                UserName = userName,
                Action = activity,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
