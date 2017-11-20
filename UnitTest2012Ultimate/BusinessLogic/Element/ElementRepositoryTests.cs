
namespace UnitTest2012Ultimate.BusinessLogic.Element
{
    using System.Collections.Generic;
    using System.Data;

    using global::BusinessLogic.Accounts.Elements;
    using global::BusinessLogic.CurrentUser;
    using global::BusinessLogic.Databases;
    using global::BusinessLogic.DataConnections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SQLDataAccess.Elements;

    using Utilities.Cryptography;

    /// <summary>
    /// Tests the Element Repository
    /// </summary>
    [TestClass]
    public class ElementRepositoryTests
    {
        [TestMethod]
        public void ElementBaseRepositoryTest()
        {
            var elementRepository = new TestElementRepository();

            var result = elementRepository[0];
            Assert.IsNull(result);

            var element = new Element(1,1,"TestElement", "TestElementDescription",false,false,false,"TestFriendlyName",false);

            result = elementRepository.Add(element);
            Assert.IsTrue(((Element)result).Id == 1);
            Assert.IsTrue(result.Description == "TestElementDescription");

            result = elementRepository[1];
            Assert.IsTrue(((Element)result).Id == 1);
            Assert.IsTrue(result.Description == "TestElementDescription");
        }

        /// <summary>
        /// Tests the SQL aspect of the Element Repository
        /// </summary>
        [TestMethod]
        public void SqlElementRepositoryTest()
        {
            var cryptography = new Mock<ICryptography>();
            cryptography.SetupAllProperties();
            var databaseCatalogue = new DatabaseCatalogue(
                new DatabaseServer(1, "localhost"),
                "test",
                "test",
                "test",
                cryptography.Object);

            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();

            var dataParameters = new Mock<DataParameters>();
            dataParameters.SetupAllProperties();

            var elementfriendly = "TestElementFriendly";
            var elementfriendly2 = "TestElementFriendly2";

            var testData = new DataTable();
            testData.Columns.Add("elementID", typeof(int));
            testData.Columns.Add("CategoryID", typeof(int));
            testData.Columns.Add("elementName");
            testData.Columns.Add("description");
            testData.Columns.Add("accessRolesCanEdit", typeof(bool));
            testData.Columns.Add("accessRolesCanAdd", typeof(bool));
            testData.Columns.Add("accessRolesCanDelete", typeof(bool));
            testData.Columns.Add("elementFriendlyName");
            testData.Columns.Add("AccessRolesApplicable", typeof(bool));

            var dataRow = testData.NewRow();
            dataRow[0] = "1";
            dataRow[1] = "1";
            dataRow[2] = "TestElement1";
            dataRow[3] = "TestElementDesc1";
            dataRow[4] = false;
            dataRow[5] = false;
            dataRow[6] = false;
            dataRow[7] = elementfriendly;
            dataRow[8] = false;
            testData.Rows.Add(dataRow);

            dataRow = testData.NewRow();
            dataRow[0] = "2";
            dataRow[1] = "2";
            dataRow[2] = "TestElement2";
            dataRow[3] = "TestElementDesc2";
            dataRow[4] = false;
            dataRow[5] = false;
            dataRow[6] = false;
            dataRow[7] = elementfriendly2;
            dataRow[8] = false;
            testData.Rows.Add(dataRow);

            var dataconnection = new TestDataConnection(dataParameters.Object, testData);
            var elementsFactory = new SqlElementFactory(dataconnection);

            //test get by Id
            IElement result = elementsFactory[1];


            //ID comes back as 0?
            Assert.IsTrue(result.Id == 1);
            Assert.IsTrue(result.FriendlyName == elementfriendly);

            result = elementsFactory[2];
            Assert.IsTrue(result.Id == 2);
            Assert.IsTrue(result.FriendlyName == elementfriendly2);

            result = elementsFactory["TestElement1"];
            Assert.IsTrue(result.Id == 1);
            Assert.IsTrue(result.FriendlyName == elementfriendly);

            result = elementsFactory["TestElement2"];
            Assert.IsTrue(result.Id == 2);
            Assert.IsTrue(result.FriendlyName == elementfriendly2);
        }
    }

    public class TestElementRepository : ElementRepository
    {
        /// <summary>
        /// Gets the list of <see cref="IElement">IElement</see>
        /// </summary>
        /// <returns>
        /// list of <see cref="IElement">IElement</see>
        /// </returns>
        public override List<IElement> Get()
        {
            return null;
        }

        /// <summary>
        /// Gets the Element by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The <see cref="IElement">IElement</see></returns>
        public override IElement this[string name]
        {
            get
            {
                return null;
            }
        }
    }
}
