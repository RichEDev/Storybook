namespace Spend_Management.shared.webServices
{
    using System.Web.Services;
    using SpendManagementLibrary.Cards;

    /// <summary>
    /// Summary description for svcCorporateCards
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcCorporateCardsProviders : WebService
    {
        /// <summary>
        /// Deletes a croporate card
        /// </summary>
        /// <param name="cardid">Id of a corporate card</param>
        /// <returns>A bool to say if he delete was successful or failed</returns>
        [WebMethod(EnableSession = true)]
        public bool DeleteCard(int cardid)
        {
            ICurrentUser user = cMisc.GetCurrentUser();
            var clscards = new CorporateCards(user.AccountID);
            return clscards.DeleteCorporateCard(cardid, user);
        }
    }
}
