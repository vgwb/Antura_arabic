using Antura.Database;
using System;
using UnityEngine;

namespace Antura.Core.Services.Notification
{
    public class NotificationService
    {
        private NotificationBridge_Interface pluginBridge;

        public NotificationService()
        {
#if (UNITY_IPHONE && !UNITY_EDITOR)
            pluginBridge = (NotificationBridge_Interface)new NotificationBridge_iOS();
#elif (UNITY_ANDROID && !UNITY_EDITOR)
            pluginBridge = (NotificationBridge_Interface)new NotificationBridge_Android();
#else
            pluginBridge = (NotificationBridge_Interface)new NotificationBridge_Editor();
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
            ScheduleSimpleWithAppIcon(
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

        /// <summary>
        /// Schedule simple notification without app icon.
        /// </summary>
        /// <param name="smallIcon">List of build-in small icons: notification_icon_bell (default), notification_icon_clock, notification_icon_heart, notification_icon_message, notification_icon_nut, notification_icon_star, notification_icon_warning.</param>
        public int ScheduleSimple(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0)
        {
            return pluginBridge.ScheduleNotification(new NotificationParams {
                Id = new System.Random().Next(),
                Delay = delay,
                Title = title,
                Message = message,
                Ticker = message,
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = smallIcon,
                SmallIconColor = smallIconColor,
                LargeIcon = ""
            });
        }

        /// <summary>
        /// Schedule notification with app icon.
        /// </summary>
        /// <param name="smallIcon">List of build-in small icons: notification_icon_bell (default), notification_icon_clock, notification_icon_heart, notification_icon_message, notification_icon_nut, notification_icon_star, notification_icon_warning.</param>
        public int ScheduleSimpleWithAppIcon(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0)
        {
            return pluginBridge.ScheduleNotification(new NotificationParams {
                Id = new System.Random().Next(),
                Delay = delay,
                Title = title,
                Message = message,
                Ticker = message,
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = smallIcon,
                SmallIconColor = smallIconColor,
                LargeIcon = "app_icon"
            });
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
            Debug.Log("NotificationService:DeleteNextLocalNotifications()");
            pluginBridge.CancelAllNotifications();
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

            pluginBridge.ScheduleNotification(notificationParams);
        }
    }
}