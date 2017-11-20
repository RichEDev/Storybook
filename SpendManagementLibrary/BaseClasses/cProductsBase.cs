using System;
using System.Web;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;

namespace SpendManagementLibrary
{
	/// <summary>
	/// Product Base Class
	/// </summary>
	public abstract class cProductsBase
	{
		/// <summary>
		/// Collection of currently defined products
		/// </summary>
		protected SortedList<int, cProduct> slProducts;
		/// <summary>
		/// Current customer account id
		/// </summary>
		protected int nAccountId;
		/// <summary>
		/// Current active subaccount id
		/// </summary>
		protected int? nSubAccountId;
		/// <summary>
		/// Product licences collection
		/// </summary>
		protected cProductLicencesBase licences;
		/// <summary>
		/// Current database connectionstring
		/// </summary>
		protected string connectionstring;

		#region properties
		/// <summary>
		/// Get a copy of the cProductBase class
		/// </summary>
		public cProductsBase getBase
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Database ID of customer account
		/// </summary>
		public int AccountID
		{
			get { return nAccountId; }
		}
		/// <summary>
		/// Database partition location ID
		/// </summary>
		public int? SubAccountID
		{
			get { return nSubAccountId; }
		}
		/// <summary>
		/// Number of categories currently defined
		/// </summary>
		public int Count
		{
			get { return slProducts.Count; }
		}

		/// <summary>
		/// Get SQL for cGridNew
		/// </summary>
		public string getGridSQL
		{
            get { return "select [ProductId],[ProductName], [ProductDescription], productCategories.[description] from productDetails"; }
		}
		#endregion

		/// <summary>
		/// cProductsBase: Collection of product definitions
		/// </summary>
		/// <param name="accountid">AccountID</param>
        /// <param name="subaccountid">Subaccount id if partition in use</param>
		public cProductsBase(int accountid, int? subaccountid)
		{
            nSubAccountId = subaccountid;
			nAccountId = accountid;
		}

		/// <summary>
		/// cProductsBase: Collection of product definitions
		/// </summary>
		/// <param name="accountid">AccountID</param>
		public cProductsBase(int accountid)
		{
			nAccountId = accountid;
		}

		/// <summary>
		/// getProductLicences: Retrieves any licences defined for the requested product ID
		/// </summary>
		/// <param name="productid">ID of the product to obtain licences for</param>
		/// <returns>A collection of product licence entities</returns>
		public cProductLicencesBase getProductLicences(int productid)
		{
			return licences;
		}

		/// <summary>
		/// UpdateProduct: Update a product record
		/// </summary>
		/// <param name="product">Product class entity to update</param>
		/// <param name="userid">ID of the user requesting the update</param>
		/// <returns>Product ID of the record</returns>
		public int UpdateProduct(cProduct product, int userid)
		{
			int retProductId;

			DBConnection db = new DBConnection(connectionstring);

			db.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
			db.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;
			db.sqlexecute.Parameters.AddWithValue("@productId", product.ProductId);
			db.sqlexecute.Parameters.AddWithValue("@subAccountId", SubAccountID.Value);
			db.sqlexecute.Parameters.AddWithValue("@productName", product.ProductName);
			db.sqlexecute.Parameters.AddWithValue("@description", product.ProductDescription);
			db.sqlexecute.Parameters.AddWithValue("@productCode", product.ProductCode);

            if (product.ProductCategory != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@productCategoryId", product.ProductCategory.ID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@productCategoryId", DBNull.Value);
            }

			db.sqlexecute.Parameters.AddWithValue("@archived", product.Archived);
			db.sqlexecute.Parameters.AddWithValue("@userid", userid);
			db.ExecuteProc("saveProduct");

			retProductId = (int)db.sqlexecute.Parameters["@ReturnId"].Value;

			db.sqlexecute.Parameters.Clear();

			return retProductId;
		}

		/// <summary>
		/// DeleteProduct: Permenently delete product record
		/// </summary>
		/// <param name="productid">ID of the product to delete</param>
		/// <param name="userid">ID of the user requesting the deletion</param>
		public void DeleteProduct(int productid, int userid)
		{
			DBConnection db = new DBConnection(connectionstring);

			db.sqlexecute.Parameters.AddWithValue("@productId", productid);
			db.sqlexecute.Parameters.AddWithValue("@userId", userid);
			db.sqlexecute.Parameters.Clear();
			return;
		}

		/// <summary>
		/// GetListControlItems: Gets a list item array of products for populating a drop down list
		/// </summary>
		/// <param name="addNonSelection">True will include a [None] option with zero value</param>
		/// <param name="sorted">True will sort collection by product name</param>
		/// <returns>List Item Array of products and product IDs</returns>
		public System.Web.UI.WebControls.ListItem[] GetListControlItems(bool addNonSelection, bool sorted)
		{
			List<System.Web.UI.WebControls.ListItem> newItem = new List<System.Web.UI.WebControls.ListItem>();
			SortedList<string, cProduct> sortedItems = new SortedList<string, cProduct>();

			if (sorted)
			{
				foreach (KeyValuePair<int, cProduct> i in slProducts)
				{
					cProduct prod = (cProduct)i.Value;
					sortedItems.Add(prod.ProductName + "_" + prod.ProductId.ToString(), prod);
				}

				foreach (KeyValuePair<string, cProduct> p in sortedItems)
				{
					cProduct prod = (cProduct)p.Value;
					newItem.Add(new System.Web.UI.WebControls.ListItem(prod.ProductName, prod.ProductId.ToString()));
				}
			}
			else
			{
				foreach (KeyValuePair<int, cProduct> i in slProducts)
				{
					cProduct prod = (cProduct)i.Value;
					newItem.Add(new System.Web.UI.WebControls.ListItem(prod.ProductName, prod.ProductId.ToString()));
				}
			}

			if (addNonSelection)
			{
				newItem.Insert(0, new System.Web.UI.WebControls.ListItem("[None]", "0"));
			}

			return newItem.ToArray();
		}

		/// <summary>
		/// GetProductById: Retrieve an individual product entity by its ID
		/// </summary>
		/// <param name="productid">ID of the product to retrieve</param>
		/// <returns>A product entity in cProduct class structure</returns>
		public cProduct GetProductById(int productid)
		{
			cProduct retProduct = null;
			if (slProducts.ContainsKey(productid))
			{
				retProduct = (cProduct)slProducts[productid];
			}

			return retProduct;
		}

		/// <summary>
		/// GetProductNameById: Obtain the name of a particular product
		/// </summary>
		/// <param name="productid">ID of the product to retrieve</param>
		/// <returns>The product's name. Empty string returned if not found.</returns>
		public string GetProductNameById(int productid)
		{
			string retStr = "";
			cProduct product = GetProductById(productid);
			if (product != null)
			{
				retStr = product.ProductName;
			}

			return retStr;
		}

		/// <summary>
		/// UpdateProductStatus: Archive or reactivate a product
		/// </summary>
		/// <param name="productid">ID of product to update</param>
		/// <param name="archived">Archive status to set</param>
		/// <param name="userid">ID of user requesting the update</param>
		public void UpdateProductStatus(int productid, bool archived, int userid)
		{
			DBConnection db = new DBConnection(connectionstring);

			db.sqlexecute.Parameters.AddWithValue("@productId", productid);
			db.sqlexecute.Parameters.AddWithValue("@archived", archived);
			db.sqlexecute.Parameters.AddWithValue("@userid", userid);
			db.ExecuteProc("changeProductStatus");

			return;
		}

        /// <summary>
        /// GetProductByName: Returns a product definition located by the product's name
        /// </summary>
        /// <param name="productname">Name of the product to find</param>
        /// <returns>NULL if not found otherwise cProduct element</returns>
        public cProduct GetProductByName(string productname)
        {
            cProduct retProduct = null;

            foreach (KeyValuePair<int, cProduct> p in slProducts)
            {
                cProduct curProduct = (cProduct)p.Value;

                if (curProduct.ProductName == productname)
                {
                    retProduct = curProduct;
                    break;
                }
            }

            return retProduct;
        }
	}
}
