using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Access_Roles.MyMobileDevicesAccessRoles
{
    class MyMobileDevicesAccessRoleMethods
    {
        public static int elementID = 165;
        #region Create Mobile Devices AccessRole
        internal static int CreateMobileDevicesAccessRole(string mobileDevicesAccessRole, int claimantID, ProductType executingProduct)
        {
            int mobileDevicesAccessRoleID;
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@accessRoleID", 0);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@accessroleName", mobileDevicesAccessRole);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@description", mobileDevicesAccessRole);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@roleAccessLevel", 1);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@expenseClaimMaximumAmount", DBNull.Value);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@expenseClaimMinimumAmount", DBNull.Value);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeesCanAmendDesignatedCostCode", 0);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeesCanAmendDesignatedDepartment", 0);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeesCanAmendDesignatedProjectCode", 0);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeeID", claimantID);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@CUdelegateID", 0);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@allowWebsiteAccess", false);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@allowMobileAccess", false);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@allowApiAccess", false);
            dbex_CodedUI.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            dbex_CodedUI.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            //dbex_CodedUI.ExecuteSQL(accessroleSQL);
            dbex_CodedUI.ExecuteProc("saveAccessRole");
            mobileDevicesAccessRoleID = (int)dbex_CodedUI.sqlexecute.Parameters["@identity"].Value;
            dbex_CodedUI.sqlexecute.Parameters.Clear();
            return mobileDevicesAccessRoleID;
        }
        #endregion

        #region Set Mobile Devices Access RoleElement
        internal static void SetMobileDevicesAccessRoleElement(int mobileDevicesAccessRoleID, int allowViewAccess, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            string checkMobileDevicesElementExists = "SELECT COUNT(*) FROM accessRoleElementDetails WHERE elementID = @elementID AND roleID = @roleID";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@roleID", mobileDevicesAccessRoleID);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@elementID", elementID);

            if (dbex_CodedUI.getcount(checkMobileDevicesElementExists) == 0)
            {
                string accessroleElementSQL = " insert into accessRoleElementDetails values (@roleID,@elementID,0,0,0,@allowviewaccess)";
                dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@allowviewaccess", allowViewAccess);
                dbex_CodedUI.ExecuteSQL(accessroleElementSQL);
            }
            else
            {
                string accessroleElementSQL = " update accessRoleElementDetails set viewAccess = @allowviewaccess";
                dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@allowviewaccess", allowViewAccess);
                dbex_CodedUI.ExecuteSQL(accessroleElementSQL);
            }
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }
        #endregion

        #region Assign Claimant Mobile Devices AccessRole
        internal static void AssignClaimantMobileDevicesAccessRole(int claimantID, int mobileDevicesAccessRoleID, ProductType executingProduct)
        {
            int subAccountId = AutoTools.GetSubAccountId(executingProduct);
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            string assignaccessroleSQL = " insert into employeeAccessRoles values (@claimantID,@mobileDeviceAccessRoleID,@subAccountID)";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@claimantID", claimantID);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@mobileDeviceAccessRoleID", mobileDevicesAccessRoleID);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@subAccountID", subAccountId);
            dbex_CodedUI.ExecuteSQL(assignaccessroleSQL);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }
        #endregion

        #region Update Claimant Mobile device AccessRole
        internal static void UpdateClaimantMobiledeviceAccessRole(int roleID, int allowViewAccess, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            string updateaccessroleSQL = " Update accessRoleElementDetails set viewAccess = @access where roleID = @roleid and elementID = @elementid";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@access", allowViewAccess);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@roleID", roleID);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@elementid", elementID);
            dbex_CodedUI.ExecuteSQL(updateaccessroleSQL);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }
        #endregion

        #region Delete Mobile Devices AccessRole
        internal static void DeleteMobileDevicesAccessRole(int mobileDevicesAccessRoleID, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            string deleteaccessroleSQL = "Delete from accessRoles where roleID = @mobileDevicesAccessRoleID";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@mobileDevicesAccessRoleID", mobileDevicesAccessRoleID);
            dbex_CodedUI.ExecuteSQL(deleteaccessroleSQL);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }
        #endregion

        #region Get Administrator or Claimant EmployeeId by Username
        public static int GetEmployeeIDByUsername(string EmployeeType, ProductType executingProduct)
        {
            int employeeid;
            string username = Convert.ToString(ConfigurationManager.AppSettings[EmployeeType].ToString());

            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string strSQL = "SELECT employeeid FROM employees WHERE username = @username";
            db.sqlexecute.Parameters.AddWithValue("@username", username);
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            {
                reader.Read();
                employeeid = reader.GetInt32(0);
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();
            return employeeid;
        }
        #endregion

        #region Checks if given access role exists in the database table
        /// <summary>
        /// Checks if given access role exists in the database table if it does returns 1 else returns 0
        /// </summary>
        /// <param name="accessRole"></param>
        /// <param name="executingProduct"></param>
        /// <returns></returns>
        public static bool AccessRoleExists(string accessRole, ProductType executingProduct)
        {
            bool accessRoleExist = false;
            int accessRoleFound = 0;
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            string strSQL = "IF EXISTS (SELECT * FROM accessRoles WHERE rolename = @accessRole) SELECT 1 ELSE SELECT 0";
            db.sqlexecute.Parameters.AddWithValue("@accessRole", accessRole);
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            {
                reader.Read();
                accessRoleFound = reader.GetInt32(0);
                reader.Close();
            }
            if (accessRoleFound == 0)
            {
                accessRoleExist = false;
            }
            else
            {
                accessRoleExist = true;
            }
            db.sqlexecute.Parameters.Clear();
            return accessRoleExist;
        }
        #endregion

    }

    internal enum EmployeeType
    {
        [System.ComponentModel.DescriptionAttribute("expensesAdminUsername")]
        Administrator = 1,
        [System.ComponentModel.DescriptionAttribute("expensesClaimantUsername")]
        Claimant = 2
    }
}
