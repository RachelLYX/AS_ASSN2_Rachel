namespace AS_ASSN2_Rachel.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task SendPasswordResetLinkAsync(string email, string subject, string link);

        Task SendConfirmationLinkAsync(string email, string subject, string link);

        Task SendPasswordResetCodeAsync(string email, string subject, string code);
    }
}
