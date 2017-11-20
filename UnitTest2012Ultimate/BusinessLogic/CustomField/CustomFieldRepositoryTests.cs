using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.BusinessLogic.CustomField
{
    using System.Data;

    using CacheDataAccess.CustomFields;

    using global::BusinessLogic.Accounts;
    using global::BusinessLogic.Cache;
    using global::BusinessLogic.CurrentUser;
    using global::BusinessLogic.CustomFields;
    using global::BusinessLogic.Databases;
    using global::BusinessLogic.DataConnections;
    using global::BusinessLogic.Fields;
    using global::BusinessLogic.Fields.Type.Attributes;
    using global::BusinessLogic.Fields.Type.Base;
    using global::BusinessLogic.Interfaces;
    using global::BusinessLogic.Logging;

    using Moq;

    using SQLDataAccess.CustomFields;

    using UnitTest2012Ultimate.BusinessLogic.CustomEntityField;

    using Utilities.Cryptography;

    [TestClass]
    public class CustomFieldRepositoryTests
    {
        /// <summary>
        /// Tests the base repository for Custom Fields
        /// </summary>
        [TestMethod]
        public void CustomEntityFieldRepositoryBaseTest()
        {
            var customEntityFieldRepo = new TestCustomFieldRepository();
            var result = customEntityFieldRepo[new Guid()];
            Assert.IsNull(result);

            var fieldGuid = new Guid();
            var field = new DecimalField(
                fieldGuid,
                "DecimalField",
                "A Decimal Field",
                "Comment",
                new Guid(),
                "property",
                new FieldAttributes(),
                new Guid(),
                10,
                10);

            result = customEntityFieldRepo.Add(field);
            Assert.IsTrue(result.Id == fieldGuid);

            result = customEntityFieldRepo[fieldGuid];
            Assert.IsTrue(result.Id == fieldGuid);
            Assert.IsTrue(result == field);
        }


        [TestMethod]
        public void CacheCustomFieldRepositoryTest()
        {
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();
            var cache = new Mock<ICache>();
            cache.SetupAllProperties();
            cache.Setup(x => x.Add(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()), It.IsAny<object>())).Returns(true);
            cache.Setup(x => x.Add(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()), It.IsAny<object>(), It.IsAny<int>())).Returns(true);
            
            var fieldGuid = Guid.NewGuid();
            var field = new DecimalField(
                fieldGuid,
                "DecimalField",
                "A Decimal Field",
                "Comment",
                 Guid.NewGuid(),
                "property",
                new FieldAttributes(),
                 Guid.NewGuid(),
                10,
                10);

            cache.Setup(x => x.Get(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), fieldGuid))).Returns(field);

            var cacheCustomFieldsFactory = new CacheCustomFieldsFactory(currentUser.Object, cache.Object);

            var database1Result = cacheCustomFieldsFactory[fieldGuid];
            Assert.IsNotNull(database1Result);
            Assert.IsTrue(database1Result.Id == fieldGuid);
            Assert.IsTrue(database1Result.Description == "A Decimal Field");

            var fieldGuid2 = Guid.NewGuid();
            var field2 = new DecimalField(
                fieldGuid2,
                "DecimalField2",
                "A Decimal Field2",
                "Comment2",
                 Guid.NewGuid(),
                "property2",
                new FieldAttributes(),
                 Guid.NewGuid(),
                10,
                10);

            // update database 2 in cache
            var customField2Result = cacheCustomFieldsFactory.Add(field2);
            Assert.IsTrue(customField2Result.Id == fieldGuid2);
            Assert.IsTrue(customField2Result.Description == "A Decimal Field2");

            customField2Result = cacheCustomFieldsFactory[fieldGuid2];
            Assert.IsTrue(customField2Result.Description == "A Decimal Field2");
       }

        [TestMethod]
        public void SqlCustomFieldRepositoryTest()
        {
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();
            var logger = new Mock<ILogger>();
            logger.SetupAllProperties();
            var dataParameters = new Mock<DataParameters>();
            dataParameters.SetupAllProperties();

            var accountRepository = new Mock<AccountRepository>();
            accountRepository.SetupAllProperties();
            var cryptography = new Mock<ICryptography>();
            cryptography.SetupAllProperties();
            var databaseCatalogue = new DatabaseCatalogue(new DatabaseServer(1, "localhost"), "test", "test", "test", cryptography.Object);

            accountRepository.Setup(x => x[It.IsAny<int>()]).Returns(new global::BusinessLogic.Accounts.Account(666, databaseCatalogue));

            var testData = new DataTable();
            testData.Columns.Add("fieldid", typeof(Guid));
            testData.Columns.Add("tableid", typeof(Guid));
            testData.Columns.Add("field");
            testData.Columns.Add("fieldType");
            testData.Columns.Add("description");
            testData.Columns.Add("comment");
            testData.Columns.Add("normalview", typeof(bool));
            testData.Columns.Add("idfield", typeof(bool));
            testData.Columns.Add("viewgroupid", typeof(int));
            testData.Columns.Add("genlist", typeof(bool));
            testData.Columns.Add("width", typeof(int));
            testData.Columns.Add("cantotal", typeof(bool));
            testData.Columns.Add("printout", typeof(bool));
            testData.Columns.Add("valuelist", typeof(bool));
            testData.Columns.Add("allowimport", typeof(bool));
            testData.Columns.Add("mandatory", typeof(bool));
            testData.Columns.Add("lookuptable", typeof(Guid));     
            testData.Columns.Add("lookupfield", typeof(Guid));
            testData.Columns.Add("useforlookup", typeof(bool));
            testData.Columns.Add("workflowUpdate", typeof(bool));
            testData.Columns.Add("workflowSearch", typeof(bool));
            testData.Columns.Add("length", typeof(int));
            testData.Columns.Add("relabel", typeof(bool));
            testData.Columns.Add("relabel_param");
            testData.Columns.Add("classPropertyName");
            testData.Columns.Add("relatedTable", typeof(Guid));
            testData.Columns.Add("IsForeignKey", typeof(bool));

            var fieldId = new Guid("EDA73369-D9C8-4E02-B892-3F512C4595D9");

            var dataRow = testData.NewRow();
            dataRow[0] = fieldId;
            dataRow[1] = Guid.NewGuid();
            dataRow[2] = "TestField";
            dataRow[3] = "TestType";
            dataRow[4] = "Description";
            dataRow[5] = "Comment";
            dataRow[6] = false;
            dataRow[7] = false;
            dataRow[8] = DBNull.Value;
            dataRow[9] = false;
      
            dataRow[10] = 100;
            dataRow[11] = false;
            dataRow[12] = false;
            dataRow[13] = false;
            dataRow[14] = false;
            dataRow[15] = false;
            dataRow[16] = Guid.NewGuid();
            dataRow[17] = Guid.NewGuid();
            dataRow[18] = false;
            dataRow[19] = false;
            dataRow[20] = false;
            dataRow[21] = 100;
            dataRow[22] = false;
            dataRow[23] = "labelParam";        
            dataRow[24] = "classProperty";
            dataRow[25] = Guid.NewGuid();
            dataRow[26] = false;

            testData.Rows.Add(dataRow);
            var dataconnection = new TestDataConnection(dataParameters.Object, testData);

            var customFieldFactory = new SqlCustomFieldFactory(
                dataconnection,
                new FieldFactory(),
                new SqlCustomFieldListvalueFactory());

            var result = customFieldFactory[fieldId];
             result = customFieldFactory["TestField"];
        }
    }

    internal class TestCustomFieldRepository : CustomFieldRepository
    {
        /// <summary>
        /// Gets the <see cref="IField">IField</see> that matches the supplied field name
        /// </summary>
        /// <param name="name">the field name</param>
        /// <returns>the <see cref="IField">IField</see></returns>
        public override IField this[string name]
        {
            get
            {
                return null;
            }
        }
    }
}
