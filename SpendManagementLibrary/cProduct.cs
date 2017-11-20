using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
	/// <summary>
	/// cProduct class definition
	/// </summary>
    [Serializable]
	public class cProduct
	{
		#region properties
		/// <summary>
		/// Product ID
		/// </summary>
		private int nProductId;
		/// <summary>
		/// Product ID
		/// </summary>
		public int ProductId
		{
            set { nProductId = value; }
			get { return nProductId; }
		}
        /// <summary>
        /// SubAccountId
        /// </summary>
        private int? nSubAccountId;
        /// <summary>
        /// SubAccountId
        /// </summary>
        public int? SubAccountId
        {
            get { return nSubAccountId; }
            set { nSubAccountId = value; }
        }
		/// <summary>
		/// Product Code
		/// </summary>
		private string sProductCode;
		/// <summary>
		/// Product Code
		/// </summary>
		public string ProductCode
		{
			get { return sProductCode; }
			set { sProductCode = value; }
		}
		/// <summary>
		/// Product Name
		/// </summary>
		private string sProductName;
		/// <summary>
		/// Product Name
		/// </summary>
		public string ProductName
		{
			get { return sProductName; }
			set { sProductName = value; }
		}
		/// <summary>
		/// Product description
		/// </summary>
		private string sProductDescription;
		/// <summary>
		/// Product description
		/// </summary>
		public string ProductDescription
		{
			get { return sProductDescription; }
			set { sProductDescription = value; }
		}
		/// <summary>
		/// Product category definition
		/// </summary>
		private cProductCategory clProductCategory;
		/// <summary>
		/// Product category definition
		/// </summary>
		public cProductCategory ProductCategory
		{
			get { return clProductCategory; }
			set { clProductCategory = value; }
		}
		
		/// <summary>
		/// Is product archived
		/// </summary>
		private bool bArchived;
		/// <summary>
		/// Is product archived
		/// </summary>
		public bool Archived
		{
			get { return bArchived; }
		}
		/// <summary>
		/// Record last modified by user id
		/// </summary>
		private DateTime dtCreatedOn;
		/// <summary>
		/// Record last modified by user id
		/// </summary>
		public DateTime CreatedOn
		{
			get { return dtCreatedOn; }
		}
		/// <summary>
		/// Date record last modified
		/// </summary>
		private int nCreatedBy;
		/// <summary>
		/// Date record last modified
		/// </summary>
		public int CreatedBy
		{
			get { return nCreatedBy; }
		}
		/// <summary>
		/// Date record last modified (NULL if not modified)
		/// </summary>
		private DateTime? dtModifiedOn;
		/// <summary>
		/// Date record last modified (NULL if not modified)
		/// </summary>
		public DateTime? ModifiedOn
		{
			get { return dtModifiedOn; }
		}
		/// <summary>
		/// Last modified by (NULL if not modified)
		/// </summary>
		private int? nModifiedBy;
		/// <summary>
		/// Last modified by (NULL if not modified)
		/// </summary>
		public int? ModifiedBy
		{
			get { return nModifiedBy; }
		}
#endregion

		/// <summary>
		/// cProduct constructor
		/// </summary>
		/// <param name="productid">Product ID</param>
        /// <param name="subaccountid">Sub Account ID</param>
		/// <param name="productcode">Product Code</param>
		/// <param name="productname">Product Name</param>
		/// <param name="productdesc">Product Description</param>
		/// <param name="productcategory">Product Category</param>
		/// <param name="archived">Is product archived</param>
		/// <param name="createdon">Date product record created</param>
		/// <param name="createdby">Product created by {user id}</param>
		/// <param name="modifiedon">Date product record last modified</param>
		/// <param name="modifiedby">Product record last modified by {user id}</param>
		public cProduct(int productid, int? subaccountid, string productcode, string productname, string productdesc, cProductCategory productcategory, bool archived, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby)
		{
			nProductId = productid;
            nSubAccountId = subaccountid;
			sProductCode = productcode;
			sProductName = productname;
			sProductDescription = productdesc;
            clProductCategory = productcategory;
			bArchived = archived;
			dtCreatedOn = createdon;
			nCreatedBy = createdby;
			dtModifiedOn = modifiedon;
			nModifiedBy = modifiedby;
		}
	}
}
