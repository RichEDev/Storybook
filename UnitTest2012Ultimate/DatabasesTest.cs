using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Spend_Management;
using UnitTest2012Ultimate.DatabaseMock;

namespace UnitTest2012Ultimate
{
    [TestClass]
    public class DatabasesTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            //forcibly clear the internal cache so one test doesn't affect another
            Databases.lstDatabases = null;
        }

        [TestMethod, TestCategory("Spend Management")]
        public void DatabasesCaches()
        {
            const string sql = "SELECT databaseID, receiptpath, logopath FROM dbo.databases";
            var database = new cDatabase(1, "receiptpath", "logopath");
            var reader = Reader.MockReaderDataFromClassData(sql, new List<object> {database}).AddAlias<cDatabase>("databaseID", x=> x.DatabaseID).AddAlias<cDatabase>("receiptpath", x=>x.ReceiptPath).AddAlias<cDatabase>("logopath", x=> x.LogoPath);
            var mockDatabase = Reader.NormalDatabase(new[] {reader});
            
            var databases1 = new Databases(mockDatabase.Object);
            Assert.AreEqual("receiptpath", databases1.GetDatabaseByID(1).ReceiptPath);
            mockDatabase.Verify(d => d.GetReader(sql, CommandType.Text), Times.Once());

            var databases2 = new Databases(mockDatabase.Object);
            Assert.AreEqual("logopath", databases2.GetDatabaseByID(1).LogoPath);
            mockDatabase.Verify(d => d.GetReader(sql, CommandType.Text), Times.Once());
        }
    }
}
