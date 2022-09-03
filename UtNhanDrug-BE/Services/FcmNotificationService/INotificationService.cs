using System.Threading.Tasks;
using UtNhanDrug_BE.Models.FcmNoti;

namespace UtNhanDrug_BE.Services.FcmNotificationService
{
    public interface INotificationService
    {
        Task<ResponseModel> SendNotification(NotificationModel notificationModel);
    }
}
