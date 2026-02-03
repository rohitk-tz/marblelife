using Core.Notification.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification
{
    public interface INotificationModelFactory
    {
        EmailNotificationModelBase CreateBase(long organizationId);
        EmailNotificationModelBase CreateBaseDefault();
    }
}
