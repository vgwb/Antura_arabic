using Antura.Database;
using System;
using UnityEngine;

namespace Antura.Core.Services.Notification
{
    public class NotificationService
    {

        public NotificationService()
        {

        }

        #region main
        /// <summary>
        /// automatically call everything to setup Notifications at AppSuspended
        /// </summary>
        public void AppSuspended()
        {
            PrepareNextLocalNotification();
        }

        /// <summary>
        /// automatically restore all Notifications at AppResumed
        /// </summary>
        public void AppResumed()
        {
            DeleteAllLocalNotifications();
        }
        #endregion

        private void PrepareNextLocalNotification()
        {
            DeleteAllLocalNotifications();
            Debug.Log("Next Local Notifications prepared");
            var arabicString = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Notification_24h);
            ScheduleSimple(
                TimeSpan.FromSeconds(CalculateSecondsToTomorrow()),
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

        #region direct plugins methods

        /// <summary>
        /// Schedule notification with app icon.
        /// </summary>
        /// <param name="smallIcon">List of build-in small icons: notification_icon_bell (default), notification_icon_clock, notification_icon_heart, notification_icon_message, notification_icon_nut, notification_icon_star, notification_icon_warning.</param>
        public void ScheduleSimple(TimeSpan delay, string title, string message, Color smallIconColor0)
        {

        }

        public void DeleteAllLocalNotifications()
        {

        }
        #endregion

        #region utilities
        private int CalculateSecondsToTomorrow()
        {
            return 3600 * 20;
        }
        private int CalculateSecondsToTomorrowMidnight()
        {
            TimeSpan ts = DateTime.Today.AddDays(2).Subtract(DateTime.Now);
            return (int)ts.TotalSeconds;
        }
        #endregion

        #region tests
        public void TestCalculateSecondsToTomorrowMidnight()
        {
            Debug.Log("Tomorrows midnight is in " + CalculateSecondsToTomorrowMidnight() + " seconds");
        }
        #endregion
    }
}