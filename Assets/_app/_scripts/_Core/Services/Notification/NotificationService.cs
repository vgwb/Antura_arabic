using System;
using UnityEngine;

namespace Antura.Core.Services.Notification
{
    public class NotificationService
    {

        public void AppSuspended()
        {
            PrepareNextLocalNotification();
        }

        public void AppResumed()
        {
            DeleteNextLocalNotfiications();
        }

        private void PrepareNextLocalNotification()
        {
            Debug.Log("Next Local Notification prepared");
            NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(10), "Antura and the Letters", "Come back to play your daily session and earn new rewards!", Color.blue);
        }

        private void DeleteNextLocalNotfiications()
        {
            Debug.Log("Next Local Notifications deleted");
            NotificationManager.CancelAll();
        }


        public void ScheduleCustom()
        {
            var notificationParams = new NotificationParams {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = TimeSpan.FromSeconds(5),
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

            NotificationManager.SendNotification(notificationParams);
        }

    }
}