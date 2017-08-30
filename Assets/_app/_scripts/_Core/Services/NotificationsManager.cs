using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.Core
{
    public class NotificationsManager
    {
        public static NotificationsManager I;

        public NotificationsManager()
        {
            I = this;
        }

        public void AppSuspended()
        {
            PrepareNextLocalNotification();
        }

        public void AppResumed()
        {
            DeleteNextLocalNotfiications();
        }

        public void PrepareNextLocalNotification()
        {
            Debug.Log("Next Local Notification prepared");
        }

        public void DeleteNextLocalNotfiications()
        {
            Debug.Log("Next Local Notifications deleted");
        }

    }
}