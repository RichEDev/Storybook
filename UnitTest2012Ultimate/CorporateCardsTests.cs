using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpendManagementLibrary;
using UnitTest2012Ultimate.DatabaseMock;

namespace UnitTest2012Ultimate
{
    using SpendManagementLibrary.Cards;

    [TestClass]
    public class CorporateCardsTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            (new Utilities.DistributedCaching.Cache()).Delete(1, string.Empty, "corporatecards");
        }

        [TestMethod, TestCategory("Spend Management")]
        public void CorporateCardsCachesList()
        {
            var cardProvider = new cCardProvider(1, "visa", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 1, new DateTime(2013, 02, 01), 1);
            var corpCard = new cCorporateCard(cardProvider, false, new DateTime(2013, 02, 01), 1, new DateTime(2013, 03, 01), 1, 1, false, false, false, false);
            string strsql = "select cardproviderid, claimants_settle_bill, createdon, createdby, modifiedon, modifiedby, allocateditem, blockcash, reconciled_by_admin, singleclaim, blockunmatched from dbo.corporate_cards";

            var corpCards = new List<object> {corpCard};
            var reader = Reader.MockReaderDataFromClassData(strsql, corpCards)
                .AddAlias<cCorporateCard>("cardproviderid", c => c.cardprovider.cardproviderid)
                .AddAlias<cCorporateCard>("claimants_settle_bill", c => c.claimantsettlesbill)
                .AddAlias<cCorporateCard>("reconciled_by_admin", c => c.reconciledbyadministrator);

            var database = Reader.NormalDatabase(new[] {reader});

            var corporateCards1 = new CorporateCards(1, database.Object);
            Assert.AreEqual(1, corporateCards1.CreateDropDown().Length);
            Assert.AreEqual(1, corporateCards1.GetGrid().Rows.Count);
            database.Verify(d => d.GetReader(strsql, CommandType.Text), Times.Once());

            var corporateCards2 = new CorporateCards(1, database.Object);
            Assert.AreEqual(1, corporateCards2.CreateDropDown().Length);
            Assert.AreEqual(1, corporateCards2.GetGrid().Rows.Count);
            database.Verify(d => d.GetReader(strsql, CommandType.Text), Times.Once());
        }

        [TestMethod, TestCategory("Spend Management")]
        public void CorporateCardsGetModifiedCorpCards()
        {
            var earlyDate = new DateTime(2013, 03, 01);
            var lateDate = new DateTime(2013, 06, 01);
            var betweenEarlyAndLate = new DateTime(2013, 04, 01);
            var cardProvider1 = new cCardProvider(1, "visa", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 1, new DateTime(2013, 02, 01), 1);
            var cardProvider2 = new cCardProvider(2, "visa", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 1, new DateTime(2013, 02, 01), 1);
            var cardProvider3 = new cCardProvider(3, "visa", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 1, new DateTime(2013, 02, 01), 1);
            var corpCard1 = new cCorporateCard(cardProvider1, false, new DateTime(2013, 02, 01), 1, earlyDate, 1, 1, false, false, false, false);
            var corpCard2 = new cCorporateCard(cardProvider2, false, new DateTime(2013, 02, 01), 1, lateDate, 1, 1, false, false, false, false);
            var corpCard3 = new cCorporateCard(cardProvider3, false, lateDate, 1, null, 1, 1, false, false, false, false);
            string strsql = "select cardproviderid, claimants_settle_bill, createdon, createdby, modifiedon, modifiedby, allocateditem, blockcash, reconciled_by_admin, singleclaim, blockunmatched from dbo.corporate_cards";

            var corpCards = new List<object> {corpCard1, corpCard2, corpCard3};
            var reader = Reader.MockReaderDataFromClassData(strsql, corpCards)
                .AddAlias<cCorporateCard>("cardproviderid", c => c.cardprovider.cardproviderid)
                .AddAlias<cCorporateCard>("claimants_settle_bill", c => c.claimantsettlesbill)
                .AddAlias<cCorporateCard>("reconciled_by_admin", c => c.reconciledbyadministrator);

            var database = Reader.NormalDatabase(new[] {reader});

            var corporateCards1 = new CorporateCards(1, database.Object);

            Assert.AreEqual(2, corporateCards1.GetModifiedCorporateCards(betweenEarlyAndLate).Count);
            //should get 2 because it has a late modified date, and 3 because it has a late created date.

        }

    }
}
