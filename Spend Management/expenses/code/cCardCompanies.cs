using System;
using System.Collections.Generic;
using SpendManagementLibrary;
using System.Data.SqlClient;
using System.Data;

namespace Spend_Management
{
    /// <summary>
    /// Functional class that services the cCardCompany object
    /// </summary>
    public class cCardCompanies
    {
        private SortedList<int, cCardCompany> lstCardCompanies;
        private int nAccountID;

        /// <summary>
        /// Constructor that caches and populates the card companies
        /// </summary>
        /// <param name="AccountID"></param>
        public cCardCompanies(int AccountID)
        {
            nAccountID = AccountID;
            initializeData();
        }

        #region Properties

        /// <summary>
        /// ID of the logged in Account
        /// </summary>
        public int AccountID
        {
            get { return nAccountID; }
        }

        #endregion

        /// <summary>
        /// Method to check the cache and if it is null then the data is extracted from the database
        /// </summary>
        private void initializeData()
        {
            if (lstCardCompanies == null || lstCardCompanies.Count == 0)
            {
                lstCardCompanies = cacheList();
            }
        }

        /// <summary>
        /// Method to extract the card companies from the database and add them to the web cache
        /// </summary>
        /// <returns></returns>
        private SortedList<int, cCardCompany> cacheList()
        {
            SortedList<int, cCardCompany> cardCompanies = new SortedList<int, cCardCompany>();
            int cardCompanyID;
            string companyName, companyNumber;
            bool usedForImport;
            DateTime? CreatedOn;
            int? CreatedBy;
            DateTime? ModifiedOn;
            int? ModifiedBy;

            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

            using (SqlDataReader reader = db.GetStoredProcReader("GetCardCompanies"))
            {
                while (reader.Read())
                {
                    cardCompanyID = reader.GetInt32(0);
                    companyName = reader.GetString(1);
                    companyNumber = reader.GetString(2);
                    usedForImport = reader.GetBoolean(3);

                    if (reader.IsDBNull(4))
                    {
                        CreatedOn = null;
                    }
                    else
                    {
                        CreatedOn = reader.GetDateTime(4);
                    }

                    if (reader.IsDBNull(5))
                    {
                        CreatedBy = null;
                    }
                    else
                    {
                        CreatedBy = reader.GetInt32(5);
                    }

                    if (reader.IsDBNull(6))
                    {
                        ModifiedOn = null;
                    }
                    else
                    {
                        ModifiedOn = reader.GetDateTime(6);
                    }

                    if (reader.IsDBNull(7))
                    {
                        ModifiedBy = null;
                    }
                    else
                    {
                        ModifiedBy = reader.GetInt32(7);
                    }

                    cardCompanies.Add(cardCompanyID, new cCardCompany(cardCompanyID, companyName, companyNumber, usedForImport, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy));
                }

                reader.Close();
            }

            return cardCompanies;
        }

        /// <summary>
        /// Force an update of the cache
        /// </summary>
        private void resetCache()
        {
            lstCardCompanies = null;
            initializeData();
        }

        /// <summary>
        /// Get the card company by its unique ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public cCardCompany GetCardCompanyByID(int ID)
        {
            cCardCompany cardCompany = null;
            lstCardCompanies.TryGetValue(ID, out cardCompany);
            return cardCompany;
        }

        /// <summary>
        /// Get the card company by its number in the VCF 4 file
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public cCardCompany GetCardCompanyByNumber(string cardNumber)
        {
            foreach (cCardCompany cardComp in lstCardCompanies.Values)
            {
                if (cardComp.companyNumber == cardNumber)
                {
                    return cardComp;
                }
            }
            return null;
        }

        /// <summary>
        /// Save the card company to the database
        /// </summary>
        /// <param name="cardCompany"></param>
        /// <returns></returns>
        public int SaveCardCompany(cCardCompany cardCompany)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            cFields clsFields = new cFields(AccountID);

            db.sqlexecute.Parameters.AddWithValue("@cardCompanyID", cardCompany.cardCompanyID);
            db.AddWithValue("@companyName", cardCompany.companyName, clsFields.GetFieldSize(new Guid("d06c868f-f828-4111-be52-df93a49dda0c")));
            db.AddWithValue("@companyNumber", cardCompany.companyNumber, clsFields.GetFieldSize(new Guid("1e449110-4ade-4d95-9e2c-6eb601278285")));
            db.sqlexecute.Parameters.AddWithValue("@usedForImport", cardCompany.usedForImport);

            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@DelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@UserID", 0);
                db.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

            }

            db.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
            db.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("SaveCardCompany");
            int id = (int)db.sqlexecute.Parameters["@id"].Value;
            db.sqlexecute.Parameters.Clear();

            resetCache();

            return id;
        }

        /// <summary>
        /// Delete the card company from the database
        /// </summary>
        /// <param name="ID"></param>
        public void DeleteCardCompany(int ID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            db.sqlexecute.Parameters.AddWithValue("ID", ID);

            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@UserID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@DelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@UserID", 0);
                db.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

            }

            db.ExecuteProc("DeleteCardCompany");
            db.sqlexecute.Parameters.Clear();

            resetCache();
        }
    }
}