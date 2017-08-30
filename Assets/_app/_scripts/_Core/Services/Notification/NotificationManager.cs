using System;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using System.Linq;
#endif

namespace Antura.Core.Services.Notification
{
    public static class NotificationManager
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private const string FullClassName = "com.hippogames.simpleandroidnotifications.Controller";
        private const string MainActivityClassName = "com.unity3d.player.UnityPlayerActivity";
#endif

        /// <summary>
        /// Schedule simple notification without app icon.
        /// </summary>
        /// <param name="smallIcon">List of build-in small icons: notification_icon_bell (default), notification_icon_clock, notification_icon_heart, notification_icon_message, notification_icon_nut, notification_icon_star, notification_icon_warning.</param>
        public static int Send(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0)
        {
            return SendNotification(new NotificationParams {
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
        public static int SendWithAppIcon(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0)
        {
            return SendNotification(new NotificationParams {
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

        /// <summary>
        /// Schedule customizable notification.
        /// </summary>
        public static int SendNotification(NotificationParams notificationParams)
        {
            var p = notificationParams;
            var delayMs = (long)p.Delay.TotalMilliseconds;
#if UNITY_ANDROID && !UNITY_EDITOR
            new AndroidJavaClass(FullClassName).CallStatic("SetNotification", p.Id, delayMs, p.Title, p.Message, p.Ticker,
                p.Sound ? 1 : 0, p.Vibrate ? 1 : 0, p.Light ? 1 : 0, p.LargeIcon, GetSmallIconName(p.SmallIcon), ColotToInt(p.SmallIconColor), MainActivityClassName);
            return notificationParams.Id;
#elif UNITY_IOS && !UNITY_EDITOR
            UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification();
            DateTime now = DateTime.Now;
            DateTime fireDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddSeconds(delayMs);
            notification.fireDate = fireDate;
            notification.alertBody = p.Message;
            notification.alertAction = p.Title;
            notification.hasAction = false;

            UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notification);

            return (int)fireDate.Ticks;
#else
            Debug.LogWarning("Simple Android Notifications are not supported for current platform. Build and play this scene on android device!");
            return 0;
#endif
        }

        /// <summary>
        /// Cancel notification by id.
        /// </summary>
        public static void Cancel(int id)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            new AndroidJavaClass(FullClassName).CallStatic("CancelNotification", id);
#endif

#if UNITY_IOS && !UNITY_EDITOR
            foreach (UnityEngine.iOS.LocalNotification notif in UnityEngine.iOS.NotificationServices.scheduledLocalNotifications) 
            { 
                if ((int)notif.fireDate.Ticks == id)
                {
                    UnityEngine.iOS.NotificationServices.CancelLocalNotification(notif);
                }
            }
#endif
        }

        /// <summary>
        /// Cancel all notifications.
        /// </summary>
        public static void CancelAll()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            new AndroidJavaClass(FullClassName).CallStatic("CancelAllNotifications");
#endif

#if UNITY_IOS && !UNITY_EDITOR
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#endif
        }

        private static int ColotToInt(Color color)
        {
            var smallIconColor = (Color32)color;

            return smallIconColor.r * 65536 + smallIconColor.g * 256 + smallIconColor.b;
        }

        private static string GetSmallIconName(NotificationIcon icon)
        {
            return "anp_" + icon.ToString().ToLower();
        }
    }
}