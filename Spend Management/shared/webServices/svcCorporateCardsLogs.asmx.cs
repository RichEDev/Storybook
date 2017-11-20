namespace Spend_Management.shared.webServices
{
    using System.Web.Services;
    using SpendManagementLibrary.Cards;
    using System;
    using expenses.code;

    /// <summary>
    /// Summary description for svcCorporateCards
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcCorporateCardsLogs : WebService
    {
        /// <summary>
        /// Gets the logs of an import
        /// </summary>
        /// <param name="importId">Id of a corporate card import</param>
        /// <returns>An instance of a CorporateCardImportLog <see cref="CorporateCardImportLog"></see></returns>
        [WebMethod(EnableSession = true)]
        public CorporateCardImportLog GetCardLogs(Guid importId)
        {
            var user = cMisc.GetCurrentUser();

            var importLogs = new CorporateCardImportLogs(user.Account);

            var importLog = importLogs.GetLogs(importId);

            return importLog;
        }
    }
}
