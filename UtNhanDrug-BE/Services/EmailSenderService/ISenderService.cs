using System.Threading.Tasks;
using UtNhanDrug_BE.Models.EmailModel;

namespace UtNhanDrug_BE.Services.EmailSenderService
{
    public interface ISenderService
    {
        Task SendEmail(MessageModel message);
    }
}
