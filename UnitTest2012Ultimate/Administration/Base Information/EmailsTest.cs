using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpendManagementLibrary;
using SpendManagementLibrary.Interfaces;
using UnitTest2012Ultimate.DatabaseMock;
using Utilities.DistributedCaching;

namespace UnitTest2012Ultimate.Administration.Base_Information
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Spend_Management;

    /// <summary>
    /// Unit Tests for emails.
    /// </summary>
    [TestClass]
    public class EmailsTest
    {
        #region Additional test attributes

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        [TestCleanup()]
        public void MyTestCleanUp()
        {
            HelperMethods.ClearTestDelegateID();
        }

        #endregion


        /// <summary>
        /// Checks that it only calls the database once if nothing is saved.
        /// </summary>
        [TestMethod]
        public void CachesList()
        {
            const string messagesSql = "select messageid, message, send, subject, description, direction, sendnote, note from dbo.messages";

            var emailsList = new List<cMessage>
                {
                    new cMessage(1, true, "subject1", "message1", 1, true, "note1", "desc1"), 
                    new cMessage(2, true, "subject2", "message2", 1, true, "note2", "desc2"),
                };
            var emailsData =
                Reader.MockReaderDataFromClassData(
                    messagesSql,
                    emailsList.Cast<object>().ToList());
            
            Mock<IDBConnection> dbConnection = Reader.NormalDatabase(new[] {emailsData});

            //clear the cache
            var cache = new Cache();
            cache.Delete(1, string.Empty, "emails");

            var emails1 = new cEmails(1, dbConnection.Object);
            //the database should have been called once:
            dbConnection.Verify(d => d.GetReader(messagesSql, CommandType.Text), Times.Once());
            var email1_1 = emails1.getMessageById(1);
            var email1_2 = emails1.getMessageById(2);
            Assert.IsNotNull(email1_1);
            Assert.IsNotNull(email1_2);
            Assert.AreEqual("subject1", email1_1.subject);
            Assert.AreEqual("subject2", email1_2.subject);

            //now create a new cEmails...
            var emails2 = new cEmails(1, dbConnection.Object);
            var email2_1 = emails2.getMessageById(1);
            Assert.IsNotNull(email2_1);
            Assert.AreEqual("subject1", email2_1.subject);
            //...and check that the total calls to the DB is still only one:
            dbConnection.Verify(d => d.GetReader(messagesSql, CommandType.Text), Times.Once());

        }

        /// <summary>
        /// Checks that if you save a email, a subsequent Get retrieves the updated value
        ///  (not the old value because it's just getting it out of the cache)
        /// </summary>
        [TestMethod]
        public void SaveemailIsReflectedNewInstance()
        {
            const string messagesSql = "select messageid, message, send, subject, description, direction, sendnote, note from dbo.messages";

            var emails = new List<cMessage>
                {
                            new cMessage(1, true, "subject1", "message1", 1, true, "note1", "desc1"),
                            new cMessage(2, true, "subject2", "message2", 1, true, "note2", "desc2"),
                };
            var emailsData =
                Reader.MockReaderDataFromClassData(
                    messagesSql,
                    emails.Cast<object>().ToList());
            
            Mock<IDBConnection> dbConnection = Reader.NormalDatabase(new[] {emailsData});

            //clear the cache
            var cache = new Cache();
            cache.Delete(1, string.Empty, "emails");
            
            var emails1 = new cEmails(1, dbConnection.Object);
            var email1_1 = emails1.getMessageById(1);
            Assert.IsNotNull(email1_1);
            Assert.AreEqual("subject1", email1_1.subject);

            //it should have hit the db once by this point
            dbConnection.Verify(c => c.GetReader(messagesSql, CommandType.Text), Times.Once());

            //tell it that when a email is saved, add that email to the ones that are returned by the reader
            var mockAuditLog = new Mock<IAuditLog>();
            emails1.updateMessage(1, false, "new subject 1", "new message 1", true, "new note 1", dbConnection.Object, mockAuditLog.Object);

            //...and check it goes to the database again
            var emails2 = new cEmails(1, dbConnection.Object);
            emails2.getMessageById(1);
            dbConnection.Verify(c => c.GetReader(messagesSql, CommandType.Text), Times.Exactly(2));
        }

        /// <summary>
        /// Checks that if you save a email, a subsequent Get retrieves the updated value
        ///  (not the old value because it's just getting it out of the cache)
        /// </summary>
        [TestMethod]
        public void SaveemailIsReflectedSameInstance()
        {
            const string messagesSql = "select messageid, message, send, subject, description, direction, sendnote, note from dbo.messages";

            var emails = new List<cMessage>
                {
                            new cMessage(1, true, "subject1", "message1", 1, true, "note1", "desc1"),
                            new cMessage(2, true, "subject2", "message2", 1, true, "note2", "desc2"),
                };
            var emailsData =
                Reader.MockReaderDataFromClassData(
                    messagesSql,
                    emails.Cast<object>().ToList());
            
            Mock<IDBConnection> dbConnection = Reader.NormalDatabase(new[] {emailsData});

            //clear the cache
            var cache = new Cache();
            cache.Delete(1, string.Empty, "emails");
            
            var emails1 = new cEmails(1, dbConnection.Object);
            var email1_1 = emails1.getMessageById(1);
            Assert.IsNotNull(email1_1);
            Assert.AreEqual("subject1", email1_1.subject);

            //it should have hit the db once by this point
            dbConnection.Verify(c => c.GetReader(messagesSql, CommandType.Text), Times.Once());

            //tell it that when a email is saved, add that email to the ones that are returned by the reader
            var mockAuditLog = new Mock<IAuditLog>();
            emails1.updateMessage(1, false, "new subject 1", "new message 1", true, "new note 1", dbConnection.Object, mockAuditLog.Object);

            //...and check it goes to the database again
            emails1.getMessageById(1);
            dbConnection.Verify(c => c.GetReader(messagesSql, CommandType.Text), Times.Exactly(2));
        }
    }
}
