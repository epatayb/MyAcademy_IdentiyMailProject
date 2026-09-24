namespace IdentiyMail.Web.Services
{
    public interface IMailService
    {
        Task SendAsync(string receiverEmail, string subject, string htmlBody);
    }
}
