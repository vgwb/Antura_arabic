using Antura.Core.Services.Notification;

namespace Antura.Core.Services
{
    public class Services
    {
        public NotificationService Notifications;

        public Services()
        {
            Notifications = new NotificationService();
        }
    }
}