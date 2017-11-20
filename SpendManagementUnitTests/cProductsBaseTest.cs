using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.UI.WebControls;

namespace SpendManagementUnitTests
{
    /// <summary>
    ///This is a test class for cProductsBaseTest and is intended
    ///to contain all cProductsBaseTest Unit Tests
    ///</summary>
	[TestClass()]
	public class cProductsBaseTest
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for getGridSQL
		///</summary>
		[TestMethod()]
		public void getGridSQLTest()
		{
            Assert.Fail("Following code needs updating");// cProduct target = new cProduct(1, null, "123", "Test Product", "Product Desc here", null, false, new System.DateTime(2009, 06, 01), cGlobalVariables.EmployeeID, null, null);
			string actual;
			//actual = target.getGridSQL;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for getBase
		///</summary>
		[TestMethod()]
		public void getBaseTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			cProductsBase actual;
			actual = target.getBase;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Count
		///</summary>
		[TestMethod()]
		public void CountTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			int actual;
			actual = target.Count;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for AccountID
		///</summary>
		[TestMethod()]
		public void AccountIDTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			int actual;
			actual = target.AccountID;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for UpdateProductStatus
		///</summary>
		[TestMethod()]
		public void UpdateProductStatusTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			int productid = 0; // TODO: Initialize to an appropriate value
			bool archived = false; // TODO: Initialize to an appropriate value
			int userid = 0; // TODO: Initialize to an appropriate value
			target.UpdateProductStatus(productid, archived, userid);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for UpdateProduct
		///</summary>
		[TestMethod()]
		public void UpdateProductTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			cProduct product = null; // TODO: Initialize to an appropriate value
			int userid = 0; // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = target.UpdateProduct(product, userid);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetProductNameById
		///</summary>
		[TestMethod()]
		public void GetProductNameByIdTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			int productid = 0; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.GetProductNameById(productid);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for getProductLicences
		///</summary>
		[TestMethod()]
		public void getProductLicencesTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			int productid = 0; // TODO: Initialize to an appropriate value
			cProductLicencesBase expected = null; // TODO: Initialize to an appropriate value
			cProductLicencesBase actual;
			actual = target.getProductLicences(productid);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetProductByName
		///</summary>
		[TestMethod()]
		public void GetProductByNameTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			string productname = string.Empty; // TODO: Initialize to an appropriate value
			cProduct expected = null; // TODO: Initialize to an appropriate value
			cProduct actual;
			actual = target.GetProductByName(productname);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetProductById
		///</summary>
		[TestMethod()]
		public void GetProductByIdTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			int productid = 0; // TODO: Initialize to an appropriate value
			cProduct expected = null; // TODO: Initialize to an appropriate value
			cProduct actual;
			actual = target.GetProductById(productid);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetListControlItems
		///</summary>
		[TestMethod()]
		public void GetListControlItemsTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			bool addNonSelection = false; // TODO: Initialize to an appropriate value
			bool sorted = false; // TODO: Initialize to an appropriate value
			ListItem[] expected = null; // TODO: Initialize to an appropriate value
			ListItem[] actual;
			actual = target.GetListControlItems(addNonSelection, sorted);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		internal virtual cProductsBase CreatecProductsBase()
		{
			// TODO: Instantiate an appropriate concrete class.
			cProductsBase target = null;
			return target;
		}

		/// <summary>
		///A test for DeleteProduct
		///</summary>
		[TestMethod()]
		public void DeleteProductTest()
		{
			cProductsBase target = CreatecProductsBase(); // TODO: Initialize to an appropriate value
			int productid = 0; // TODO: Initialize to an appropriate value
			int userid = 0; // TODO: Initialize to an appropriate value
			target.DeleteProduct(productid, userid);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}
	}
}
