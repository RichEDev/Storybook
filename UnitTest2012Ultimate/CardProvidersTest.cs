using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpendManagementLibrary;
using UnitTest2012Ultimate.DatabaseMock;

namespace UnitTest2012Ultimate
{
    [TestClass]
    public class CardProvidersTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            //clear the cache to ensure one test doesn't influence another
            CardProviders.cardProviders = null;
        }

        [TestMethod, TestCategory("Spend Management")]
        public void CardProvidersCaches()
        {

            const string cardProvidersSql = "select cardproviderid, cardprovider, card_type, createdon, createdby, modifiedon, modifiedby from dbo.card_providers";
            var cardProvider = new cCardProvider(1, "visa", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);

            //set up the database to return one card provider
            var mockDatabase = Reader.NormalDatabase(new[] { Reader.MockReaderDataFromClassData(cardProvidersSql, new List<object> { cardProvider }).AddAlias<cCardProvider>("card_type", x => x.cardtype ) });

            //create a cardproviders collection, and check it returns one card provider, and has called the database:
            var cCardProviders1 = new CardProviders(mockDatabase.Object);
            Assert.AreEqual(1, cCardProviders1.getCardProviders().Count);
            mockDatabase.Verify(c => c.GetReader(cardProvidersSql, CommandType.Text), Times.Once());

            //now create another, and check it hasn't gone back to the database (still only been called once)
            var cCardProviders2 = new CardProviders(mockDatabase.Object);
            Assert.AreEqual(1, cCardProviders2.getCardProviders().Count);
            mockDatabase.Verify(c => c.GetReader(cardProvidersSql, CommandType.Text), Times.Once());

        }

        [TestMethod, TestCategory("Spend Management")]
        public void CardProvidersCreateListOfAllCards()
        {
            //check that CreateList returns all the card providers in the database, and they are in order of 'cardprovider'

            //set up the database to return 4 card provider, not in the right order
            const string cardProvidersSql = "select cardproviderid, cardprovider, card_type, createdon, createdby, modifiedon, modifiedby from dbo.card_providers";
            var cardProvider1 = new cCardProvider(1, "visa_CreditCard", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider2 = new cCardProvider(2, "mastercard_CreditCard", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider3 = new cCardProvider(3, "visa_PurchaseCard", CorporateCardType.PurchaseCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider4 = new cCardProvider(4, "mastercard_PurchaseCard", CorporateCardType.PurchaseCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var mockDatabase = Reader.NormalDatabase(new[] { Reader.MockReaderDataFromClassData(cardProvidersSql, new List<object> { cardProvider1, cardProvider2, cardProvider3, cardProvider4 }).AddAlias<cCardProvider>("card_type", x => x.cardtype) });

            var cardProviders = new CardProviders(mockDatabase.Object);
            var listItems = cardProviders.CreateList();
            CheckListItems(listItems, new[]
                {
                    new[] {"2", "mastercard_CreditCard"},
                    new[] {"4", "mastercard_PurchaseCard"},
                    new[] {"1", "visa_CreditCard"},
                    new[] {"3", "visa_PurchaseCard"}
                });
        }

        [TestMethod, TestCategory("Spend Management")]
        public void CardProvidersGetById()
        {
            //check that get by id returns the required card provider

            //set up the database to return 4 card providers
            const string cardProvidersSql = "select cardproviderid, cardprovider, card_type, createdon, createdby, modifiedon, modifiedby from dbo.card_providers";
            var cardProvider1 = new cCardProvider(1, "visa_CreditCard", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider2 = new cCardProvider(2, "mastercard_CreditCard", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider3 = new cCardProvider(3, "visa_PurchaseCard", CorporateCardType.PurchaseCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider4 = new cCardProvider(4, "mastercard_PurchaseCard", CorporateCardType.PurchaseCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var mockDatabase = Reader.NormalDatabase(new[] { Reader.MockReaderDataFromClassData(cardProvidersSql, new List<object> { cardProvider1, cardProvider2, cardProvider3, cardProvider4 }).AddAlias<cCardProvider>("card_type", x => x.cardtype) });

            var cardProviders = new CardProviders(mockDatabase.Object);

            var cardProvider = cardProviders.getProviderByID(2);
            Assert.IsNotNull(cardProvider);
            Assert.AreEqual("mastercard_CreditCard", cardProvider.cardprovider);
            Assert.AreEqual(2, cardProvider.cardproviderid);
        }

        [TestMethod, TestCategory("Spend Management")]
        public void CardProvidersGetByName()
        {
            //check that get by name returns the required card provider

            //set up the database to return 4 card providers
            const string cardProvidersSql = "select cardproviderid, cardprovider, card_type, createdon, createdby, modifiedon, modifiedby from dbo.card_providers";
            var cardProvider1 = new cCardProvider(1, "visa_CreditCard", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider2 = new cCardProvider(2, "mastercard_CreditCard", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider3 = new cCardProvider(3, "visa_PurchaseCard", CorporateCardType.PurchaseCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider4 = new cCardProvider(4, "mastercard_PurchaseCard", CorporateCardType.PurchaseCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var mockDatabase = Reader.NormalDatabase(new[] { Reader.MockReaderDataFromClassData(cardProvidersSql, new List<object> { cardProvider1, cardProvider2, cardProvider3, cardProvider4 }).AddAlias<cCardProvider>("card_type", x => x.cardtype) });

            var cardProviders = new CardProviders(mockDatabase.Object);

            var cardProvider = cardProviders.getProviderByName("mastercard_PurchaseCard");
            Assert.IsNotNull(cardProvider);
            Assert.AreEqual("mastercard_PurchaseCard", cardProvider.cardprovider);
            Assert.AreEqual(4, cardProvider.cardproviderid);
        }

        [TestMethod, TestCategory("Spend Management")]
        public void CardProvidersGetCardProviders()
        {
            //check that getcardproviders returns a dictionary of all the card providers

            //set up the database to return 4 card providers
            const string cardProvidersSql = "select cardproviderid, cardprovider, card_type, createdon, createdby, modifiedon, modifiedby from dbo.card_providers";
            var cardProvider1 = new cCardProvider(1, "visa_CreditCard", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider2 = new cCardProvider(2, "mastercard_CreditCard", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider3 = new cCardProvider(3, "visa_PurchaseCard", CorporateCardType.PurchaseCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var cardProvider4 = new cCardProvider(4, "mastercard_PurchaseCard", CorporateCardType.PurchaseCard, new DateTime(2013, 01, 01), 0, new DateTime(2013, 01, 01), 0);
            var mockDatabase = Reader.NormalDatabase(new[] { Reader.MockReaderDataFromClassData(cardProvidersSql, new List<object> { cardProvider1, cardProvider2, cardProvider3, cardProvider4 }).AddAlias<cCardProvider>("card_type", x => x.cardtype) });

            var cardProviders = new CardProviders(mockDatabase.Object);

            var dictionary = cardProviders.getCardProviders();
            
            Assert.AreEqual(4, dictionary.Count);
            Assert.AreEqual(cardProvider1.cardprovider, dictionary[1].cardprovider);
            Assert.AreEqual(cardProvider1.cardproviderid, dictionary[1].cardproviderid);
            Assert.AreEqual(cardProvider2.cardprovider, dictionary[2].cardprovider);
            Assert.AreEqual(cardProvider2.cardproviderid, dictionary[2].cardproviderid);
            Assert.AreEqual(cardProvider3.cardprovider, dictionary[3].cardprovider);
            Assert.AreEqual(cardProvider3.cardproviderid, dictionary[3].cardproviderid);
            Assert.AreEqual(cardProvider4.cardprovider, dictionary[4].cardprovider);
            Assert.AreEqual(cardProvider4.cardproviderid, dictionary[4].cardproviderid);
        }

        private void CheckListItems(ListItem[] listItems, string[][] expectedValueAndText)
        {
            Assert.AreEqual(listItems.Length, expectedValueAndText.Length, "Wrong number of list items");
            for (int i = 0; i < listItems.Length; i++)
            {
                Assert.AreEqual(expectedValueAndText[i][0], listItems[i].Value, "Value of list item {0} does not match", i);
                Assert.AreEqual(expectedValueAndText[i][1], listItems[i].Text, "Text of list item {0} does not match");
            }
        }
    }



}
