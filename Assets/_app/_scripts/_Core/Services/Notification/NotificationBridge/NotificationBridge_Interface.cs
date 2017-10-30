using System;

namespace Antura.Core.Services.Notification
{

    public interface NotificationBridge_Interface
    {
        void Test();

        int ScheduleNotification(NotificationParams notificationParams);

        void CancelNotification(int id);

        void CancelAllNotifications();
    }
}