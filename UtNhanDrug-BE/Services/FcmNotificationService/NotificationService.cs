using CorePush.Google;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UtNhanDrug_BE.Models.FcmNoti;
using static UtNhanDrug_BE.Models.FcmNoti.GoogleNotification;

namespace UtNhanDrug_BE.Services.FcmNotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly FcmNotificationSetting _fcmNotificationSetting;
        private readonly String tokenDevice = "eE2bKSwCPJt52AZaOclYpO:APA91bHlQfi1sTUz7OuEiLl_r6BMYm9oYRjkgKxUU00isxZoKVIUuTmdTq4z5WgVl1sPvZpOdCOodpo-OCwru1pPylFqFJ9D3cOuTR6WjD1ysLWsoj_ggftJmmma5DjNxiyWIbnKPTsz";
        public NotificationService(IOptions<FcmNotificationSetting> settings)
        {
            _fcmNotificationSetting = settings.Value;
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
                string deviceToken = tokenDevice;

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
