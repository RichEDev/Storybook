using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.Caching;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace SpendManagementLibrary
{
	#region cRecharge
	[Serializable()]
	public class cRecharge
	{
		[Serializable()]
		public struct RechargeElements
		{
			public double Maintenance;
			public int Quantity;
			public RechargeApportionType ApportionType;
			public double Portion;
			public SurchargeType SurchargeApportionType;
			public double SurchargePortion;
			public RechargeApportionType PostWarrantyApportionType;
			public double PostWarrantyPortion;
			public DateTime SupportStartDate;
			public DateTime WarrantyEndDate;
			public DateTime SupportEndDate;
			public double RechargeValue;
			public double UnrecoveredPercent;
			public SortedList<int,cRechargeServiceDate> ServiceDates;
		}

		private DateTime currentRechargeDate;
		private RechargeElements currentRechargeElements;
		[NonSerialized()]
		private string sConnectionString;
		private int nAccountId;
		private int? nSubAccountId;
		private int curContractId;
		private int curRechargeId;
		private int curRechargeEntityId;
		private int curContractProductId;
		private string curRechargeEntityName;
		private int curCurrencyId;
		private int curProductId;
		private string curProductName;
		private Dictionary<string, string> uf_FieldValues;

		public DropDownList CreateApportionTypeListControl(string controlID, RechargeApportionType selectedType)
		{
			DropDownList ddlist = new DropDownList();

			ddlist.ID = controlID;
			ddlist.Items.Add(new ListItem("Fixed", ((int)RechargeApportionType.Fixed).ToString()));
			ddlist.Items.Add(new ListItem("n_Units", ((int)RechargeApportionType.n_Units).ToString()));
			ddlist.Items.Add(new ListItem("Percentage", ((int)RechargeApportionType.Percentage).ToString()));

			ddlist.Items.FindByValue(((int)selectedType).ToString()).Selected = true;

			return ddlist;
		}

		public DropDownList CreateSurchargeTypeListControl(string controlID, SurchargeType selectedType)
		{
			DropDownList ddlist = new DropDownList();

			ddlist.ID = controlID;
			ddlist.Items.Add(new ListItem("Fixed", ((int)SurchargeType.Fixed).ToString()));
			ddlist.Items.Add(new ListItem("Percentage", ((int)SurchargeType.Percentage).ToString()));

			ddlist.Items.FindByValue(((int)selectedType).ToString()).Selected = true;

			return ddlist;
		}

		public string GetApportionTypeDesc()
		{
			string retStr = "";
			switch (currentRechargeElements.ApportionType)
			{
				case RechargeApportionType.Percentage:
					retStr = "Percent";
					break;
				case RechargeApportionType.Fixed:
					retStr = "Fixed";
					break;
				case RechargeApportionType.n_Units:
					retStr = "n Units";
					break;
				default:
					retStr = "Unknown";
					break;
			}
			return retStr;
		}

		public string GetPWApportionTypeDesc()
		{
			string retStr = "";
			switch (currentRechargeElements.PostWarrantyApportionType)
			{
				case RechargeApportionType.Percentage:
					retStr = "Percent";
					break;
				case RechargeApportionType.Fixed:
					retStr = "Fixed";
					break;
				case RechargeApportionType.n_Units:
					retStr = "n Units";
					break;
				default:
					retStr = "Unknown";
					break;
			}
			return retStr;
		}

		public string GetSurchargeTypeDesc()
		{
			string retStr = "";
			switch (SurchargeApportionType)
			{
				case SurchargeType.Percentage:
					retStr = "Percent";
					break;
				case SurchargeType.Fixed:
					retStr = "Fixed";
					break;
				default:
					retStr = "Unknown";
					break;
			}
			return retStr;
		}

		#region properties
		public int AccountID
		{
			get { return nAccountId; }
		}
		public int? SubAccountId
		{
			get { return nSubAccountId; }
		}
		public int ProductId
		{
			get { return curProductId; }
			set { curProductId = value; }
		}
		public string ProductName
		{
			get { return curProductName; }
			set { curProductName = value; }
		}

		public int CurrencyId
		{
			get { return curCurrencyId; }
			set { curCurrencyId = value; }
		}

		public string RechargeEntityName
		{
			get { return curRechargeEntityName; }
			set { curRechargeEntityName = value; }
		}

		public int RechargeId
		{
			get { return curRechargeId; }
			set { curRechargeId = value; }
		}

		public int RechargeEntityId
		{
			get { return curRechargeEntityId; }
			set { curRechargeEntityId = value; }
		}

		public int ContractId
		{
			get { return curContractId; }
			set { curContractId = value; }
		}

		public int ContractProductId
		{
			get { return curContractProductId; }
			set { curContractProductId = value; }
		}

		private double SetRechargeValue
		{
			set { currentRechargeElements.RechargeValue = value; }
		}

		public double Maintenance
		{
			get { return currentRechargeElements.Maintenance; }
			set { currentRechargeElements.Maintenance = value; }
		}

		public int Quantity
		{
			get { return currentRechargeElements.Quantity; }
			set { currentRechargeElements.Quantity = value; }
		}

		public double GetRechargeValue
		{
			get { return currentRechargeElements.RechargeValue; }
		}

		public DateTime SupportStartDate
		{
			get { return currentRechargeElements.SupportStartDate; }
			set { currentRechargeElements.SupportStartDate = value; }
		}

		public DateTime WarrantyEndDate
		{
			get { return currentRechargeElements.WarrantyEndDate; }
			set { currentRechargeElements.WarrantyEndDate = value; }
		}

		public DateTime SupportEndDate
		{
			get { return currentRechargeElements.SupportEndDate; }
			set { currentRechargeElements.SupportEndDate = value; }
		}

		public double UnrecoveredPercent
		{
			get { return currentRechargeElements.UnrecoveredPercent; }
			set { currentRechargeElements.UnrecoveredPercent = value; }
		}

		public DateTime SetCurrentRechargeDate
		{
			set { currentRechargeDate = value; }
		}

		public RechargeApportionType ApportionType
		{
			get { return currentRechargeElements.ApportionType; }
			set { currentRechargeElements.ApportionType = value; }
		}

		public RechargeApportionType PostWarrantyApportionType
		{
			get { return currentRechargeElements.PostWarrantyApportionType; }
			set { currentRechargeElements.PostWarrantyApportionType = value; }
		}

		public SurchargeType SurchargeApportionType
		{
			get { return currentRechargeElements.SurchargeApportionType; }
			set { currentRechargeElements.SurchargeApportionType = value; }
		}

		public double Portion
		{
			get { return currentRechargeElements.Portion; }
			set { currentRechargeElements.Portion = value; }
		}

		public double SurchargePortion
		{
			get { return currentRechargeElements.SurchargePortion; }
			set { currentRechargeElements.SurchargePortion = value; }
		}

		public double PostWarrantyPortion
		{
			get { return currentRechargeElements.PostWarrantyPortion; }
			set { currentRechargeElements.PostWarrantyPortion = value; }
		}

		public SortedList<int,cRechargeServiceDate> ServiceDates
		{
			get { return currentRechargeElements.ServiceDates; }
			set { currentRechargeElements.ServiceDates = value; }
		}

		public Dictionary<string, string> UserFieldValues
		{
			get { return uf_FieldValues; }
			set { uf_FieldValues = value; }
		}
		#endregion

		public double CalcRechargeValue()
		{
			double RechargeResult;

			try
			{
				DBConnection db = new DBConnection(sConnectionString);

				string sql = "SELECT dbo.CalcRechargeValue(@curRechargeDate, @RA_Id) AS [CalcValue]";
				db.sqlexecute.Parameters.AddWithValue("@RA_Id", curRechargeId);
				db.sqlexecute.Parameters.AddWithValue("@curRechargeDate", currentRechargeDate);
				RechargeResult = (double)db.getSum(sql);

			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Error calculating recharge value\n" + ex.Message);
				RechargeResult = 0;
			}

			SetRechargeValue = RechargeResult;
			return RechargeResult;
		}

		public bool SetRechargeProperties(DataRow RechargeRecord, SortedList<int, cRechargeServiceDate> servicedates, Dictionary<int,cUserdefinedFieldGrouping> ufgrps)
		{
			//try
			//{
			//    // if any field is not present, then it will return false
			//    currentRechargeElements.Portion = (double)RechargeRecord["Portion"];
			//    currentRechargeElements.ApportionType = (RechargeApportionType)RechargeRecord["Apportion Id"];
			//    if ((RechargeRecord["Support End Date"]) != DBNull.Value)
			//    {
			//        currentRechargeElements.SupportEndDate = (DateTime)RechargeRecord["Support End Date"];
			//    }
			//    else
			//    {
			//        currentRechargeElements.SupportEndDate = DateTime.MinValue;
			//    }

			//    if (RechargeRecord["Support Start Date"] != DBNull.Value)
			//    {
			//        currentRechargeElements.SupportStartDate = (DateTime)RechargeRecord["Support Start Date"];
			//    }
			//    else
			//    {
			//        currentRechargeElements.SupportStartDate = DateTime.MinValue;
			//    }

			//    if (RechargeRecord["Warranty End Date"] != DBNull.Value)
			//    {
			//        currentRechargeElements.WarrantyEndDate = (DateTime)RechargeRecord["Warranty End Date"];
			//    }
			//    else
			//    {
			//        currentRechargeElements.WarrantyEndDate = DateTime.MinValue;
			//    }

			//    currentRechargeElements.SurchargeApportionType = (SurchargeType)RechargeRecord["Surcharge Type"];
			//    currentRechargeElements.SurchargePortion = (double)RechargeRecord["Surcharge"];
			//    currentRechargeElements.PostWarrantyApportionType = (RechargeApportionType)RechargeRecord["Post Warranty Apportion Id"];
			//    currentRechargeElements.PostWarrantyPortion = (double)RechargeRecord["Post Warranty Portion"];
			//    currentRechargeElements.Quantity = (int)RechargeRecord["Quantity"];
			//    currentRechargeElements.Maintenance = (double)RechargeRecord["Maintenance Value"];
			//    currentRechargeElements.UnrecoveredPercent = (double)RechargeRecord["Unrecovered"];
			//    currentRechargeElements.ServiceDates = servicedates;
			//    ContractId = (int)RechargeRecord["Contract Id"];
			//    RechargeId = (int)RechargeRecord["Recharge Id"];
			//    RechargeEntityId = (int)RechargeRecord["Recharge Entity Id"];
			//    ContractProductId = (int)RechargeRecord["Contract-Product Id"];
			//    RechargeEntityName = (string)RechargeRecord["Name"];
			//    CurrencyId = (int)RechargeRecord["Currency Id"];
			//    ProductName = (string)RechargeRecord["Product Name"];
			//    ProductId = (int)RechargeRecord["Product Id"];

			//    foreach (KeyValuePair<int,cUserdefinedFieldGrouping> x in ufgrps)
			//    {
			//        cUserdefinedFieldGrouping ufgrp = (cUserdefinedFieldGrouping)x.Value;

			//        foreach (KeyValuePair<int, cUserDefinedField> i in ufields)
			//        {
			//            cUserDefinedField ufield = (cUserDefinedField)i.Value;
			//            string fieldVal = "";
			//            string fieldid = "UF" + ufield.FieldId.ToString();

			//            switch (ufield.FieldType)
			//            {
			//                case UserFieldType.Character:
			//                case UserFieldType.Text:
			//                case UserFieldType.DDList:
			//                    if (RechargeRecord[fieldid] != DBNull.Value)
			//                    {
			//                        fieldVal = (string)RechargeRecord[fieldid];
			//                    }
			//                    break;
			//                case UserFieldType.DateField:
			//                    if (RechargeRecord[fieldid] != DBNull.Value)
			//                    {
			//                        DateTime dateVal = (DateTime)RechargeRecord[fieldid];
			//                        fieldVal = dateVal.ToShortDateString();
			//                    }
			//                    break;
			//                case UserFieldType.Float:
			//                    if (RechargeRecord[fieldid] != DBNull.Value)
			//                    {
			//                        double floatVal = (double)RechargeRecord[fieldid];
			//                        fieldVal = floatVal.ToString();
			//                    }
			//                    break;
			//                case UserFieldType.Number:
			//                    if (RechargeRecord[fieldid] != DBNull.Value)
			//                    {
			//                        int intVal = (int)RechargeRecord[fieldid];
			//                        fieldVal = intVal.ToString();
			//                    }
			//                    break;
			//                case UserFieldType.StaffName_Ref:
			//                case UserFieldType.Site_Ref:
			//                case UserFieldType.RechargeClient_Ref:
			//                case UserFieldType.RechargeAcc_Code:
			//                    if (RechargeRecord[fieldid] != DBNull.Value)
			//                    {
			//                        int refVal = (int)RechargeRecord[fieldid];
			//                        fieldVal = refVal.ToString();
			//                    }
			//                    break;
			//                default:
			//                    break;
			//            }

			//            if (uf_FieldValues == null)
			//            {
			//                uf_FieldValues = new Dictionary<string, string>();
			//            }

			//            if (!uf_FieldValues.ContainsKey(fieldid))
			//            {
			//                uf_FieldValues.Add(fieldid, fieldVal);
			//            }
			//        }
			//   }

			//    return true;
			//}
			//catch (Exception ex)
			//{
			//    System.Diagnostics.Debug.WriteLine("cRecharge:SetRechargeProperties:" + ex.Message);
			//    return false;

			//}

			throw new NotImplementedException();
		}

		private double GetBasicCharge(int numDaysInMonth, int NumDays)
		{
			try
			{
				double curValue = 0;

				switch (currentRechargeElements.ApportionType)
				{
					case RechargeApportionType.Fixed:
						curValue = currentRechargeElements.Portion;
						break;
					case RechargeApportionType.n_Units:
						curValue = ((currentRechargeElements.Maintenance / 12) / currentRechargeElements.Quantity) * currentRechargeElements.Portion;
						break;
					case RechargeApportionType.Percentage:
						curValue = ((currentRechargeElements.Maintenance / 12) / 100) * currentRechargeElements.Portion;
						break;
					default:
						break;
				}
				if (numDaysInMonth > 0 && NumDays > 0)
				{
					// only return a proportion of the monthly charge
					double retVal;
					retVal = (curValue / numDaysInMonth) * NumDays;
					curValue = retVal;
				}

				// apply surcharge
				curValue = ApplySurcharge(curValue);

				return curValue;
			}
			catch
			{
				return 0;
			}
		}

		private double GetPostWarrantyCharge(int numDaysInMonth, int NumDays)
		{
			try
			{
				double curValue = 0;

				switch (currentRechargeElements.PostWarrantyApportionType)
				{
					case RechargeApportionType.Percentage:
						curValue = ((currentRechargeElements.Maintenance / 12) / 100) * currentRechargeElements.PostWarrantyPortion;
						break;
					case RechargeApportionType.Fixed:
						curValue = currentRechargeElements.PostWarrantyPortion;
						break;
					case RechargeApportionType.n_Units:
						curValue = ((currentRechargeElements.Maintenance / 12) / currentRechargeElements.Quantity) * currentRechargeElements.PostWarrantyPortion;
						break;
					default:
						break;
				}

				if (numDaysInMonth > 0 && NumDays > 0)
				{
					// only return a proportion of the monthly charge
					double retVal;
					retVal = (curValue / numDaysInMonth) * NumDays;
					curValue = retVal;
				}

				// apply surcharge
				curValue = ApplySurcharge(curValue);

				return curValue;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("cRecharge:GetPostWarrantyCharge:" + ex.Message);
				return 0;
			}
		}

		private double ApplySurcharge(double curValue)
		{
			try
			{
				double tmpValue;

				switch (currentRechargeElements.SurchargeApportionType)
				{
					case SurchargeType.Fixed:
						tmpValue = curValue + currentRechargeElements.SurchargePortion;
						break;
					case SurchargeType.Percentage:
						tmpValue = curValue + ((curValue / 100) * currentRechargeElements.SurchargePortion);
						break;
					default:
						tmpValue = curValue;
						break;
				}

				return tmpValue;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("cRecharge:ApplySurcharge:" + ex.Message);
				return curValue;
			}
		}

		public string GetUserFieldValue(string UF_FieldName)
		{
			string retField = null;
			if (uf_FieldValues.ContainsKey(UF_FieldName))
			{
				retField = (string)uf_FieldValues[UF_FieldName];
			}

			return retField;
		}

		public void UpdateUserFieldValue(string UF_FieldName, string Value)
		{
			if (uf_FieldValues.ContainsKey(UF_FieldName))
			{
				uf_FieldValues[UF_FieldName] = Value;
			}
			return;
		}

		public cRecharge(int accountid, int? subaccountid, string connStr)
		{
			sConnectionString = connStr;
			nAccountId = accountid;
			nSubAccountId = subaccountid;
		}
	}
	#endregion

	#region cRechargeClient class
	[Serializable()]
	public class cRechargeClient
	{
		public cRechargeClient(int LocId, int EntityId, string ClientName, bool isShared, int StaffRep, int DeputyRep, int AccMgr, int ServiceMgr, string Notes, bool isClosed, DateTime DateClosed, DateTime DateCeased, string Code, string ServiceLine, string Sector)
		{
			nLocationId = LocId;
			nEntityId = EntityId;
			sClientName = ClientName;
			bShared = isShared;
			nStaffRepId = StaffRep;
			nDeputyRepId = DeputyRep;
			nAccountManagerId = AccMgr;
			nServiceManagerId = ServiceMgr;
			sNotes = Notes;
			bClosed = isClosed;
			dDateClosed = DateClosed;
			dDateCeased = DateCeased;
			sCode = Code;
			sSector = Sector;
			sServiceLine = ServiceLine;
		}

		public int EntityId
		{
			get { return nEntityId; }
		}
		private int nEntityId;

		public int LocationId
		{
			get { return nLocationId; }
		}
		private int nLocationId;

		public string ClientName
		{
			get { return sClientName; }
		}
		private string sClientName;
		public int AccountManagerId
		{
			get { return nAccountManagerId; }
		}
		private int nAccountManagerId;

		public int StaffRepId
		{
			get { return nStaffRepId; }
		}
		private int nStaffRepId;

		public int DeputyRepId
		{
			get { return nDeputyRepId; }
		}
		private int nDeputyRepId;

		public int ServiceManagerId
		{
			get { return nServiceManagerId; }
		}
		private int nServiceManagerId;

		public bool isShared
		{
			get { return bShared; }
		}
		private bool bShared;

		public string Notes
		{
			get { return sNotes; }
		}
		private string sNotes;

		public DateTime DateCeased
		{
			get { return dDateCeased; }
		}
		private DateTime dDateCeased;

		public DateTime DateClosed
		{
			get { return dDateClosed; }
		}
		private DateTime dDateClosed;

		public bool isClosed
		{
			get { return bClosed; }
		}
		private bool bClosed;

		public string Code
		{
			get { return sCode; }
		}
		private string sCode;

		public string ServiceLine
		{
			get { return sServiceLine; }
		}
		private string sServiceLine;

		public string Sector
		{
			get { return sSector; }
		}
		private string sSector;
	}
	#endregion

	#region Recharge Payment Class
	[Serializable()]
	public class cRechargePayment
	{
		private int nContractId;
		public int ContractId
		{
			get { return nContractId; }
		}

		private int nRechargeId;
		public int RechargeId
		{
			get { return nRechargeId; }
		}

		private int nCurrencyId;
		public int CurrencyId
		{
			get { return nCurrencyId; }
			set { nCurrencyId = value; }
		}

		private int nPaymentId;
		public int PaymentId
		{
			get { return nPaymentId; }
		}

		private DateTime dtPaymentDate;
		public DateTime PaymentDate
		{
			get { return dtPaymentDate; }
			set { dtPaymentDate = value; }
		}

		//public bool IsOneOffCharge
		//{
		//    get { return bIsOneOffCharge; }
		//}
		//private bool bIsOneOffCharge;

		private int nEntityId;
		public int EntityId
		{
			get { return nEntityId; }
			set { nEntityId = value; }
		}

		private string sEntityName;
		public string EntityName
		{
			get { return sEntityName; }
			set { sEntityName = value; }
		}

		private double dAmount;
		public double Amount
		{
			get { return dAmount; }
			set { dAmount = value; }
		}

		private int nContractProductId;
		public int ContractProductId
		{
			get { return nContractProductId; }
			set { nContractProductId = value; }
		}

		private int nProductId;
		public int ProductId
		{
			get { return nProductId; }
			set { nProductId = value; }
		}

		private string sProductName;
		public string ProductName
		{
			get { return sProductName; }
			set { sProductName = value; }
		}

		private string sCPUF1_Value;
		public string CPUF1_Value
		{
			get { return sCPUF1_Value; }
		}

		private int nCPUF1_Id;
		public int CPUF1_Id
		{
			get { return nCPUF1_Id; }
		}

		private string sCPUF2_Value;
		public string CPUF2_Value
		{
			get { return sCPUF2_Value; }
		}

		private int nCPUF2_Id;
		public int CPUF2_Id
		{
			get { return nCPUF2_Id; }
		}

		public cRechargePayment(int contractId, int contractCurrencyId, int rechargepaymentId, int rechargeId, DateTime paymentDate, int rechargeEntityId, string rechargeEntityName, int rechargeProductId, string rechargeProductName, int rechargeConProdId, double rechargeAmount, int CPUF1, string CPUF1Value, int CPUF2, string CPUF2Value) //, bool isOneOffCharge
		{
			nContractId = contractId;
			nCurrencyId = contractCurrencyId;
			nPaymentId = rechargepaymentId;
			nRechargeId = rechargeId;
			dtPaymentDate = paymentDate;
			nEntityId = rechargeEntityId;
			sEntityName = rechargeEntityName;
			dAmount = rechargeAmount;
			nProductId = rechargeProductId;
			sProductName = rechargeProductName;
			nContractProductId = rechargeConProdId;
			//bIsOneOffCharge = isOneOffCharge;
			nCPUF1_Id = CPUF1;
			sCPUF1_Value = CPUF1Value;
			nCPUF2_Id = CPUF2;
			sCPUF2_Value = CPUF2Value;
		}
	}
	#endregion

	#region Recharge Account Code class
	public class cRechargeAccountCode
	{
		private int nLocationId;
		private int nCodeId;
		private string sAccountCode;
		private string sDescription;
		private cContractCategory conCategory;

		public int LocationId
		{
			get { return nLocationId; }
		}

		public int CodeId
		{
			get { return nCodeId; }
		}

		public string AccountCode
		{
			get { return sAccountCode; }
		}
		public string Description
		{
			get { return sDescription; }
		}
		public cContractCategory Category
		{
			get { return conCategory; }
		}

		public cRechargeAccountCode(int codeid, int locationid, string accountcode, string description, cContractCategory contractcategory)
		{
			nCodeId = codeid;
			nLocationId = locationid;
			sAccountCode = accountcode;
			sDescription = description;
			conCategory = contractcategory;
		}
	}
	#endregion

	#region cRechargeServiceDate
	[Serializable()]
	public class cRechargeServiceDate
	{
		private int nServiceDateId;
		public int ServiceDateId
		{
			get { return nServiceDateId; }
		}

		private int nRechargeAssociationId;
		public int RechargeId
		{
			get { return nRechargeAssociationId; }
		}

		private DateTime dOfflineFrom;
		public DateTime OfflineFrom
		{
			get { return dOfflineFrom; }
		}

		private DateTime dOnlineFrom;
		public DateTime OnlineFrom
		{
			get { return dOnlineFrom; }
		}

		public cRechargeServiceDate(int servicedateid, int RAid, DateTime offlinefrom, DateTime onlinefrom)
		{
			nServiceDateId = servicedateid;
			nRechargeAssociationId = RAid;
			dOfflineFrom = offlinefrom;
			dOnlineFrom = onlinefrom;
		}

	}
#endregion
}
