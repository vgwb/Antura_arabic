using Antura.Core.Services.Notification;
using Antura.Core.Services.Gallery;
using Antura.Core.Services.OnlineAnalytics;

namespace Antura.Core.Services
{
    public class ServicesManager
    {
        public NotificationService Notifications;
        public GalleryService Gallery;
        public AnalyticsService Analytics;

        public ServicesManager()
        {
            Notifications = new NotificationService();
            Gallery = new GalleryService();
            Analytics = new AnalyticsService();
        }
    }
}