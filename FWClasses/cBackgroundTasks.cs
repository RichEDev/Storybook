using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SpendManagementLibrary;
using Spend_Management;

namespace FWClasses
{
    public class cBackgroundTasks
    {
        public delegate bool TerminateClient(int clientId, DateTime supportenddate);
        public delegate void GeneratePayments(int contractId, string[] items, DateTime curPeriodDate, DateTime genEndDate, int rcPeriodMonths);
        public delegate int GeneratePaymentsForContract(int contractId, DateTime curPeriodDate, DateTime genEndDate, int rcPeriodMonths);
		private CurrentUser curUser;
		private cAccountProperties properties;

        public cBackgroundTasks(CurrentUser currentUser)
        {
			curUser = currentUser;
			cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
			if (curUser.CurrentSubAccountId >= 0)
			{
				properties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
			}
			else
			{
				properties = subaccs.getFirstSubAccount().SubAccountProperties;
			}
        }

        public int GenerateRechargePaymentsForContract(int contractId, DateTime curPeriodDate, DateTime genEndDate, int rcPeriodMonths)
        {
			cEmployees employees = new cEmployees(curUser.AccountID);

			cRechargeCollection rcoll = new cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId,curUser.EmployeeID, contractId, cAccounts.getConnectionString(curUser.AccountID), properties);
            SortedList items = rcoll.GetCollection;
            string comma = "";
            StringBuilder csvItems = new StringBuilder();

            for (int i = 0; i < items.Count; i++)
            {
                cRecharge curItem = (cRecharge)items.GetByIndex(i);
                if (curItem != null)
                {
                    csvItems.Append(comma);
                    csvItems.Append(curItem.RechargeId);
                    comma = ",";
                }
            }

            if (csvItems.Length > 0)
            {
                GenerateRechargePayments(contractId, csvItems.ToString().Split(','), curPeriodDate, genEndDate, rcPeriodMonths);
            }

            return contractId;
        }

        public void GenerateRechargePayments(int contractId, string[] items, DateTime curPeriodDate, DateTime genEndDate, int rcPeriodMonths)
        {
			cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
			cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);

			cAccountProperties accProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;

            string sql = "";
            cFWDBConnection db = new cFWDBConnection();

            db.DBOpen(fws, false);
            DateTime curGenDate;
			cEmployees employees = new cEmployees(curUser.AccountID);

			cCPFieldInfo CP_UFInfoColl = new cCPFieldInfo(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, contractId);
            
            for (int i = 0; i < items.Length; i++)
            {
                cCurrencies currency = new cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId);
				cProducts products = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
                int curContractCurrencyId = 0;
                db.FWDb("R", "contract_details", "ContractId", contractId, "", "", "", "", "", "", "", "", "", "");
                if (db.FWDb2Flag)
                {
                    int.TryParse(db.FWDbFindVal("ContractCurrency", 2), out curContractCurrencyId);
                }
                
				cRechargeCollection rc = new cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, contractId, cAccounts.getConnectionString(curUser.AccountID), properties);
                cRecharge rechargeItem = rc.GetRechargeItemById(int.Parse(items[i]));
				cRechargePaymentCollection rPayments = new cRechargePaymentCollection(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID), CP_UFInfoColl, contractId, products, curPeriodDate);
               
                rPayments.DeleteRechargePaymentForPeriod(rechargeItem.RechargeId, curPeriodDate, genEndDate);

                // delete any record already existing before recreating it with new values
                sql = "DELETE FROM [contract_productdetails_recharge] WHERE [RechargeId] = @rId AND [RechargePeriod] BETWEEN @rpstart AND @rpend AND [OneOffCharge] = 0";
                db.AddDBParam("rId", rechargeItem.RechargeId, true);
                db.AddDBParam("rpstart", curPeriodDate, false);
                db.AddDBParam("rpend", genEndDate, false);
                db.ExecuteSQL(sql.ToString());

                curGenDate = curPeriodDate;

                while (curGenDate <= genEndDate)
                {
                    // create entries according to the template

                    // only generate a payment if within the support dates
                    rechargeItem.SetCurrentRechargeDate = curGenDate;

                    if (rechargeItem.CalcRechargeValue() > 0)
                    {
                        db.SetFieldValue("RechargeId", rechargeItem.RechargeId, "N", true);
                        db.SetFieldValue("RechargePeriod", curGenDate.ToShortDateString(), "D", false);
                        db.SetFieldValue("RechargeAmount", rechargeItem.GetRechargeValue, "N", false);
                        db.SetFieldValue("RechargeEntity Id", rechargeItem.RechargeEntityId, "N", false);
                        db.SetFieldValue("ContractProductId", rechargeItem.ContractProductId, "N", false);

                        db.FWDb("W", "contract_productdetails_recharge", "", "", "", "", "", "", "", "", "", "", "", "");
                        int newPaymentId = db.glIdentity;

                        db.FWDb("R2", "contract_productdetails", "ContractProductId", rechargeItem.ContractProductId, "", "", "", "", "", "", "", "", "", "");
                        string UF1Value = "";
                        string UF2Value = "";
                        if (db.FWDb2Flag)
                        {
                            UF1Value = db.FWDbFindVal("UF" + CP_UFInfoColl.CPFieldInfoItem.CP_UF1.ToString(), 2);
                            UF2Value = db.FWDbFindVal("UF" + CP_UFInfoColl.CPFieldInfoItem.CP_UF2.ToString(), 2);
                        }

                        db.FWDb("R3", "contract_details", "ContractId", contractId, "", "", "", "", "", "", "", "", "", "");
                        if (db.FWDb3Flag)
                        {
                            if (db.FWDbFindVal("ContractCurrency", 3) != "")
                            {
                                curContractCurrencyId = int.Parse(db.FWDbFindVal("Contract Currency", 3));
                            }
                        }
                        cRechargePayment rPayment = new cRechargePayment(contractId, curContractCurrencyId, newPaymentId, rechargeItem.RechargeId, curGenDate, rechargeItem.RechargeEntityId, rechargeItem.RechargeEntityName, rechargeItem.ProductId, products.GetProductNameById(rechargeItem.ProductId), rechargeItem.ContractProductId, rechargeItem.GetRechargeValue, CP_UFInfoColl.CPFieldInfoItem.CP_UF1, UF1Value, CP_UFInfoColl.CPFieldInfoItem.CP_UF2, UF2Value);
                        rPayments.AddRechargePayment(rPayment);
                    }

                    curGenDate = curGenDate.AddMonths(rcPeriodMonths);
                }
            }

            db.DBClose();
            return;
        }

        public bool SetClientTermination(int clientid, DateTime supportenddate)
        {
            try
            {
                int curContractId = 0;
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT DISTINCT contract_productdetails.[ContractId] FROM [recharge_associations] ");
                sql.Append("INNER JOIN contract_productdetails ON recharge_associations.[ContractProductId] = contract_productdetails.[ContractProductId] ");
                sql.Append("WHERE [RechargeEntityId] = @clientid");

                cFWDBConnection db = new cFWDBConnection();

                ArrayList checkContracts = new ArrayList();
				cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
				cFWSettings fws = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId);
                db.DBOpen(fws, false);

                db.AddDBParam("clientid", clientid, true);
                System.Data.SqlClient.SqlDataReader reader;
                using (reader = db.GetReader(sql.ToString()))
                {
                    while (reader.Read())
                    {
                        curContractId = reader.GetInt32(reader.GetOrdinal("ContractId"));

                        if (!checkContracts.Contains(curContractId))
                        {
                            checkContracts.Add(curContractId);
                        }
                    }
                    reader.Close();
                }

                for (int i = 0; i < checkContracts.Count; i++)
                {
					cEmployees employees = new cEmployees(curUser.AccountID);
					cRechargeCollection rtemplates = new cRechargeCollection(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, (int)checkContracts[i], fws.getConnectionString, properties);
                    ArrayList itemArray = rtemplates.GetRechargeItemsByEntity(clientid);
                    StringBuilder csvUpdateIdList = new StringBuilder();
                    string comma = "";
                                        
                    for (int idx = 0; idx < itemArray.Count; idx++)
                    { 
                        // set the support end date for each client template item
                        cRecharge rItem = (cRecharge)itemArray[idx];

                        rItem.SupportEndDate = supportenddate;

                        csvUpdateIdList.Append(comma);
                        csvUpdateIdList.Append(rItem.RechargeId.ToString());
                        comma = ",";
                    }

                    if (csvUpdateIdList.Length != 0)
                    {
                        // update db entries
                        DateTime terminationDate = DateTime.Parse("01/" + supportenddate.Month.ToString() + "/" + supportenddate.Year.ToString());

                        db.AddDBParam("SEDate", terminationDate, true);
                        db.ExecuteSQL("UPDATE recharge_associations SET [supportEndDate] = @SEDate WHERE [rechargeId] IN (" + csvUpdateIdList.ToString() + ")");

                        // remove any recharge payments generated beyond the end date
                        cProducts products = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
						cRechargePaymentCollection rpayments = new cRechargePaymentCollection(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID), new cCPFieldInfo(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID), curUser.EmployeeID, (int)checkContracts[i]), (int)checkContracts[i], products);
                        rpayments.DeleteRechargePaymentByClient(clientid, terminationDate.AddMonths(1), DateTime.MaxValue);

                        db.ExecuteSQL("DELETE FROM contract_productdetails_recharge WHERE [rechargeId] IN (" + csvUpdateIdList.ToString() + ") AND [rechargePeriod] = @SEDate");

                        // force re-generation of payments for the last month (only if currently generated).
                        StringBuilder paymentsql = new StringBuilder();
                        paymentsql.Append("SELECT [rechargeId], dbo.CalcRechargeValue(@SEDate,[rechargeId]) AS [RechargeValue] FROM recharge_associations ");
                        paymentsql.Append("WHERE [rechargeId] in (" + csvUpdateIdList.ToString() + ")");

                        db.AddDBParam("SEDate", terminationDate, true);
                        db.RunSQL(paymentsql.ToString(), db.glDBWorkA, false, "", false);

                        foreach (DataRow drow in db.glDBWorkA.Tables[0].Rows)
                        {
                            int rechargeId = (int)drow["RechargeId"];
                            double rechargeValue = (double)drow["RechargeValue"];

                            ArrayList arrPayments = rpayments.GetClientPaymentsBetween(clientid, terminationDate, terminationDate.AddDays(10));

                            for (int x = 0; x < arrPayments.Count; x++)
                            {
                                cRechargePayment p = (cRechargePayment)arrPayments[x];

                                if (p.RechargeId == rechargeId)
                                {
                                    // only want to update payment if it already exists
                                    p.Amount = rechargeValue;

                                    db.SetFieldValue("rechargeAmount", rechargeValue, "N", true);
                                    db.FWDb("A", "contract_productdetails_recharge", "rechargeItemId", p.PaymentId, "", "", "", "", "", "", "", "", "", "");
                                }
                            }
                        }
                    }
                }

                db.DBClose();

                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
