namespace SpendManagementLibrary.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface for cEmailTemplates
    /// </summary>
    public interface IEmailTemplates
    {
        void SendMessage(int emailtemplateid, cField entityfield, int filterval, int senderid, List<sSendDetails> lstRecipientTypes, string bodyUpdate);
    }
}
