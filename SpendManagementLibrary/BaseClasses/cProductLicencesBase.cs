using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SpendManagementLibrary
{
	/// <summary>
	/// Base class for product definition
	/// </summary>
	public abstract class cProductLicencesBase
	{
		protected int nProductId;
		protected int nAccountId;
        protected int nSubAccountId;
        protected int nEmployeeId;
		protected Dictionary<int, cProductLicence> licences;
		protected cProductsBase products;
		protected cLicenceRenewalType renewalTypes;
		protected string sConnectionString;
        protected cAccountProperties accProperties;

		#region properties
		/// <summary>
		/// Database ID of customer account
		/// </summary>
		public int AccountID
		{
			get { return nAccountId; }
		}
		/// <summary>
		/// Database partition subaccount ID
		/// </summary>
		public int SubAccountID
		{
			get { return nSubAccountId; }
		}

        /// <summary>
        /// Current employee ID
        /// </summary>
        public int EmployeeID
        {
            get { return nEmployeeId; }
        }

		/// <summary>
		/// Number of licences currently defined
		/// </summary>
		public int Count
		{
			get { return licences.Count; }
		}
		/// <summary>
		/// Database ID of product 'owning' the licences
		/// </summary>
		public int ProductID
		{
			get { return nProductId; }
		}
		/// <summary>
		/// Returns the current collection of licences
		/// </summary>
		public Dictionary<int, cProductLicence> GetCollection
		{
			get { return licences; }
		}

        /// <summary>
        /// Connection string to access the DB
        /// </summary>
        public string connectionString
        {
            get { return sConnectionString; }
        }
		#endregion

		public cProductLicencesBase(int accountid, int subaccountid, int employeeid, int productid, string connectionString)
		{
            sConnectionString = connectionString;
			nProductId = productid;
            nSubAccountId = subaccountid;
			nAccountId = accountid;
            nEmployeeId = EmployeeID;
		}

		private void UpdateLicenceCount(int productId)
		{
			bool updateCopies = true;
            
				if (!accProperties.AutoUpdateLicenceTotal)
				{
					updateCopies = false;
				}

			if (updateCopies)
			{
				string sql = "UPDATE productDetails SET [NumLicencedCopies] = (SELECT SUM([NumberCopiesHeld]) FROM productLicences WHERE [ProductId] = @productId) WHERE [ProductId] = @productId";
                DBConnection db = new DBConnection(connectionString);
				
				db.sqlexecute.Parameters.AddWithValue("@productId", productId);
				db.ExecuteSQL(sql);
			}
		}

		/// <summary>
		/// GetLicenceById: Get individual licence instance by its ID
		/// </summary>
		/// <param name="licenceid">ID of licence you wish to retrieve</param>
		/// <returns>A licence entity or NULL if not found</returns>
		public cProductLicence GetLicenceById(int licenceid)
		{
			if (licences.ContainsKey(licenceid))
			{
				return (cProductLicence)licences[licenceid];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// UpdateLicence: Update an individual licence entity record
		/// </summary>
		/// <param name="newlicence">Licence record in a cProductLicence class structure</param>
		/// <returns>Database ID of the product licence</returns>
		public virtual int UpdateLicence(cProductLicence newlicence)
		{
			DBConnection db = new DBConnection(connectionString);

			db.sqlexecute.Parameters.AddWithValue("@licenceId", newlicence.LicenceId);
			db.sqlexecute.Parameters.AddWithValue("@productId", newlicence.ProductId);
			db.sqlexecute.Parameters.AddWithValue("@licenceKey", newlicence.LicenceKey);

            if (newlicence.LicenceType > 0)
            {
                db.sqlexecute.Parameters.AddWithValue("@licenceType", newlicence.LicenceType);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@licenceType", DBNull.Value);
            }
			
			db.sqlexecute.Parameters.AddWithValue("@location", newlicence.LicenceLocation);
			if (newlicence.LicenceExpiry == DateTime.MinValue)
			{
				db.sqlexecute.Parameters.AddWithValue("@expiry", DBNull.Value);
			}
			else
			{
				db.sqlexecute.Parameters.AddWithValue("@expiry", newlicence.LicenceExpiry);
			}
			db.sqlexecute.Parameters.AddWithValue("@notifyDays", newlicence.NotifyDays);
			db.sqlexecute.Parameters.AddWithValue("@notifyId", newlicence.NotifyId);
			db.sqlexecute.Parameters.AddWithValue("@notifyType", (int)newlicence.NotifyType);
			db.sqlexecute.Parameters.AddWithValue("@softCopy", newlicence.IsElectronicCopyHeld);
			db.sqlexecute.Parameters.AddWithValue("@hardCopy", newlicence.IsHardCopyHeld);
			db.sqlexecute.Parameters.AddWithValue("@unlimited", newlicence.IsUnlimitedLicence);
			db.sqlexecute.Parameters.AddWithValue("@numberCopiesHeld", newlicence.NumberCopiesHeld);

            if (newlicence.LicenceRenewalType != null)
            {
                if (newlicence.LicenceRenewalType.ID > 0)
                {
                    db.sqlexecute.Parameters.AddWithValue("@renewalType", newlicence.LicenceRenewalType.ID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@renewalType", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@renewalType", DBNull.Value);
            }

            db.sqlexecute.Parameters.Add("@ReturnId", System.Data.SqlDbType.Int);
            db.sqlexecute.Parameters["@ReturnId"].Direction = System.Data.ParameterDirection.ReturnValue;

            bool updateCount = accProperties.AutoUpdateLicenceTotal;

            db.sqlexecute.Parameters.AddWithValue("@updateCount", updateCount);

			db.sqlexecute.Parameters.AddWithValue("@userid", EmployeeID);
			db.ExecuteProc("saveProductLicence");

			int licenceId = (int)db.sqlexecute.Parameters["@ReturnId"].Value;

			return licenceId;
		}

		/// <summary>
		/// DeleteLicence: Permenently delete a product licence record
		/// </summary>
		/// <param name="licenceid">ID of the licence record to delete</param>
		/// <param name="userid">ID of the user requesting the deletion</param>
		public virtual void DeleteLicence(int licenceid, int userid)
		{
			if (licences.ContainsKey(licenceid))
			{
				//cProducts products = new cProducts(fws, uinfo);

				//AuditRoutines.AuditLog ALog = new AuditRoutines.AuditLog();
				//AuditRoutines.AuditRecord ARec = new AuditRoutines.AuditRecord();
				//ARec.Action = AuditRoutines.AUDIT_DEL;
				//ARec.DataElementDesc = "PRODUCT LICENCE";
				//ARec.ElementDesc = products.GetProductById(licence.ProductId).ProductName.ToUpper();
				//ARec.PreVal = licence.LicenceKey;
				//ARec.PostVal = "";
				//ALog = AuditRoutines.AddAuditRec(ALog, ARec, true);
				//ALog = AuditRoutines.CommitAuditLog(fws, ALog, uinfo);

				DBConnection db = new DBConnection(connectionString);

				// Check the AUTOUPDATE_PRODUCT_LICENCES param for whether to auto sum
				bool updateCount = accProperties.AutoUpdateLicenceTotal;

				db.sqlexecute.Parameters.AddWithValue("@licenceId", licenceid);
				db.sqlexecute.Parameters.AddWithValue("@userid", userid);
				db.sqlexecute.Parameters.AddWithValue("@updateCount", updateCount);
				db.ExecuteProc("deleteProductLicence");
			}
			return;
		}
	}
}
