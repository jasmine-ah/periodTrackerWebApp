using System.Threading.Tasks;


namespace periodTracker.Services
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData);
    }
}

