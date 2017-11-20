using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;

namespace Expenses_Scheduler
{
    class cRecurringPurchaseOrders
    {
        private int nAccountID;
        private List<int> lstPurchaseOrdersToCreate;

        public cRecurringPurchaseOrders(int accountID)
        {
            nAccountID = accountID;

            lstPurchaseOrdersToCreate = FindPurchaseOrdersToCreate();
        }

        private List<int> FindPurchaseOrdersToCreate()
        {
            List<int> lstPurchaseOrdersToCreate = new List<int>();
            DBConnection smdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
            //smdata.sqlexecute.Parameters.AddWithValue("@orderType", (int)PurchaseOrderType.Recurring);
            //smdata.sqlexecute.Parameters.AddWithValue("@orderEndDate", DateTime.Today.ToString("dd/MM/yyyy"));
            //string strSQL = "SELECT purchaseOrderID FROM dbo.purchaseOrders WHERE orderType=@orderType AND orderEndDate=@orderEndDate";

            string strsql;
            System.Data.SqlClient.SqlDataReader reader;
            DateTime startdate = new DateTime(1900, 01, 01, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            DateTime enddate = startdate.AddMinutes(15);
            // where does purchaseOrderRecurringScheduleDays etc come in to it??
            strsql = "select purchaseOrderId from purchaseOrderRecurringSchedules where ((getDate() between startdate and enddate) or (getdate() > startdate and enddate is null))and (starttime between @startdate and @enddate)";
            smdata.sqlexecute.Parameters.AddWithValue("@startdate", startdate);
            smdata.sqlexecute.Parameters.AddWithValue("@enddate", enddate);

			using (reader = smdata.GetReader(strsql))
			{
				smdata.sqlexecute.Parameters.Clear();

				int purchaseOrderId;
				while (reader.Read())
				{
					purchaseOrderId = reader.GetInt32(1);

					lstPurchaseOrdersToCreate.Add(purchaseOrderId);
				}
				reader.Close();
			}
            return lstPurchaseOrdersToCreate;
        }

        #region properties
        public List<int> purchaseOrderIds
        {
            get
            {
                return lstPurchaseOrdersToCreate;
            }
        }

        #endregion properties
    }
}
