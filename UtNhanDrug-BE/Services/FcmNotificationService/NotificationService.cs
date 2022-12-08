using CorePush.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.FcmNoti;
using System.Linq;
using static UtNhanDrug_BE.Models.FcmNoti.GoogleNotification;

namespace UtNhanDrug_BE.Services.FcmNotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly FcmNotificationSetting _fcmNotificationSetting;
        private readonly ut_nhan_drug_store_databaseContext _context;
        //private readonly String tokenDevice = "eE2bKSwCPJt52AZaOclYpO:APA91bHlQfi1sTUz7OuEiLl_r6BMYm9oYRjkgKxUU00isxZoKVIUuTmdTq4z5WgVl1sPvZpOdCOodpo-OCwru1pPylFqFJ9D3cOuTR6WjD1ysLWsoj_ggftJmmma5DjNxiyWIbnKPTsz";
        //private readonly String tokenDevice1 = "eE2bKSwCPJt52AZaOclYpO:APA91bHxtV41prYkvAXIKjumNUoAIpgAnFOV47BMp2zqQFzckzepn43VqQGTjhzg1apcuu8zviTR8CEj4NwO3jhMfTXua-a8MmcigdfcsspstIbF1z6uw9axMj0EZO9Ye2Eh62MJADoi";
        public NotificationService(IOptions<FcmNotificationSetting> settings, ut_nhan_drug_store_databaseContext context)
        {
            _fcmNotificationSetting = settings.Value;
            _context = context;
        }
        public async Task<ResponseModel> SendNotification(NotificationModel notificationModel)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                /* FCM Sender*/
                FcmSettings settings = new FcmSettings()
                {
                    SenderId = _fcmNotificationSetting.SenderId,
                    ServerKey = _fcmNotificationSetting.ServerKey
                };
                HttpClient httpClient = new HttpClient();

                string authorizationKey = string.Format("keyy={0}", settings.ServerKey);
                var query = from m in _context.Managers
                            select m;
                string deviceToken = await query.Select(x => x.FcmToken).FirstOrDefaultAsync();

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                httpClient.DefaultRequestHeaders.Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                DataPayload dataPayload = new DataPayload()
                {
                    Title = notificationModel.Title,
                    Body = notificationModel.Body
                };
                GoogleNotification notification = new GoogleNotification()
                {
                    Data = dataPayload,
                    Notification = dataPayload
                };

                var fcm = new FcmSender(settings, httpClient);
                var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);

                if (fcmSendResponse.IsSuccess())
                {
                    response.IsSuccess = true;
                    response.Message = "Notification sent successfully";
                    return response;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = fcmSendResponse.Results[0].Error;
                    return response;
                }
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Something went wrong";
                return response;
            }
        }
    }
}
