using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cCorporateCardObject
    {
        /// <summary>
        /// Get the global corporate card object    
        /// </summary>
        /// <returns></returns>
        public static cCorporateCard GetCorporateCard(string provider)
        {
            cCorporateCard card = new cCorporateCard(GetCardProvider(provider), false, DateTime.UtcNow, 0, null, null, null, false, false, true, false);
            return card;
        }

        /// <summary>
        /// Get a template card provider object
        /// </summary>
        /// <returns></returns>
        public static cCardProvider GetCardProvider(string provider)
        {
            cCardProviders clsCardProviders = new cCardProviders();
            cCardProvider prov = clsCardProviders.getProviderByName(provider);
            return prov;
        }

        /// <summary>
        /// Create and save the corporate card object to the database and return
        /// </summary>
        /// <returns></returns>
        public static cCorporateCard CreateCorporateCard(string CardProvider)
        {
            cCorporateCards clsCorpCards = new cCorporateCards(cGlobalVariables.AccountID);
            cCorporateCard card = GetCorporateCard(CardProvider);

            clsCorpCards.addCorporateCard(card);

            SortedList<string, cCorporateCard> lstCorpCards = clsCorpCards.sortList();

            cCorporateCard tempCard = null;
            lstCorpCards.TryGetValue(card.cardprovider.cardprovider, out tempCard);

            cGlobalVariables.CorporateCardID = tempCard.cardprovider.cardproviderid;
            return tempCard;
        }

        /// <summary>
        /// Delete the corprate card from the database
        /// </summary>
        /// <param name="ID"></param>
        public static void DeleteCorporateCard(int ID)
        {
            cCorporateCards clsCorpCards = new cCorporateCards(cGlobalVariables.AccountID);
            clsCorpCards.deleteCorporateCard(ID);
        }
    }
}
