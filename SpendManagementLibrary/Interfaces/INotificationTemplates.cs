namespace SpendManagementLibrary.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface for NotificationTemplates
    /// </summary>
    public interface INotificationTemplates
    {
        void SendMessage(int emailtemplateid, cField entityfield, int filterval, int senderid, List<sSendDetails> lstRecipientTypes, string bodyUpdate);
    }
}
