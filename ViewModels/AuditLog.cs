namespace AS_ASSN2_Rachel.ViewModels
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string UserName { get; set; }
        public string Action { get; set; }

        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
