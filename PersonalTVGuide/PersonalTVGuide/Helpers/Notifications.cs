using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PersonalTVGuide.Helpers;

namespace System.Web.Mvc
{
    public static class Notifications
    {
        public static IEnumerable<Notification> listNotifications = new List<Notification> { 
            new Notification {
                NotificationId = 0,
                Name = "Dagelijks"
            },
            new Notification {
                NotificationId = 1,
                Name = "Wekelijks"
            }
        };
    }
}