using Antura.Core.Services.Notification;
using Antura.Core.Services.Gallery;

namespace Antura.Core.Services
{
    public class ServicesManager
    {
        public NotificationService Notifications;
        public GalleryService Gallery;

        public ServicesManager()
        {
            Notifications = new NotificationService();
            Gallery = new GalleryService();
        }
    }
}