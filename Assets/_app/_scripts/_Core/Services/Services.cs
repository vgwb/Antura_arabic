using Antura.Core.Services.Notification;

namespace Antura.Core.Services
{
    public class ServicesManager
    {
        public NotificationService Notifications;

        public ServicesManager()
        {
            Notifications = new NotificationService();
        }
    }
}