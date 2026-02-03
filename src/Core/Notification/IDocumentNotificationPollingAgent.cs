using System;
using System.Collections.Generic;

namespace Core.Notification
{
    public interface IDocumentNotificationPollingAgent
    {
        void CreateDocumentUploadNotification(string fileName, ICollection<long> franciseeIds, long? createdBy);
        void SendExpiryNotification();
    }
}
