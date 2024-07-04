using Notifications.Wpf;

namespace Gerencia_Reportes.Helpers
{
    public static class NotifiactionMessage
    {


        private static readonly NotificationManager notificationManager = new NotificationManager();


        public static void SetMessage(string title, string message, NotificationType type)
        {
            var content = new NotificationContent
            {
                Title = title,
                Message = message,
                Type = type
            };
            notificationManager.Show(content);
        }

    }
}
