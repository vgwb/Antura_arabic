using Antura.Database;
using System;
using UnityEngine;

#if UNITY_IOS
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#endif

namespace Antura.Core.Services.Notification
{
    public class NotificationService
    {
        public NotificationService()
        {
#if UNITY_IOS
            NotificationServices.RegisterForNotifications(
                NotificationType.Alert |
                NotificationType.Badge |
                NotificationType.Sound
            );
            Debug.Log("Registered for Notifications");
#endif
        }

        public void AppSuspended()
        {
            PrepareNextLocalNotification();
        }

        public void AppResumed()
        {
            DeleteNextLocalNotifications();
        }

        private void PrepareNextLocalNotification()
        {
            Debug.Log("Next Local Notifications prepared");
            var arabicString = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Notification_24h);
            NotificationManager.ScheduleSimpleWithAppIcon(
                TimeSpan.FromSeconds(CalculateSecondsToTomorrowMidnight()),
                "Antura and the Letters",
                arabicString.Arabic,
                Color.blue
            );

            //NotificationManager.ScheduleSimpleWithAppIcon(
            //    TimeSpan.FromSeconds(60),
            //    "Antura and the Letters",
            //    "Test notification after closing the app [60 seconds]",
            //    Color.blue
            //);
        }

        public void TestCalculateSecondsToTomorrowMidnight()
        {
            Debug.Log("Tomorrows midnight is in " + CalculateSecondsToTomorrowMidnight() + " seconds");
        }

        private int CalculateSecondsToTomorrowMidnight()
        {
            TimeSpan ts = DateTime.Today.AddDays(2).Subtract(DateTime.Now);
            return (int)ts.TotalSeconds;
        }

        private void DeleteNextLocalNotifications()
        {
            Debug.Log("Next Local Notifications deleted");
            NotificationManager.CancelAllNotifications();
        }


        public void ScheduleCustomNotification()
        {
            var notificationParams = new NotificationParams {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = TimeSpan.FromSeconds(60),
                Title = "Custom notification",
                Message = "Message",
                Ticker = "Ticker",
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = NotificationIcon.Heart,
                SmallIconColor = new Color(0, 0.5f, 0),
                LargeIcon = "app_icon"
            };

            NotificationManager.ScheduleNotification(notificationParams);
        }

    }
}