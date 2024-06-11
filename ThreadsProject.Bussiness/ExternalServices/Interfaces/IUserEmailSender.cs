namespace ThreadsProject.Bussiness.ExternalServices.Interfaces
{
    public interface IUserEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
