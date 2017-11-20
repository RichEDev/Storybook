using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cSupplier
    {
        int supplierid;
        int nSubAccountId;
        string suppliername;
        cSupplierStatus cssSupplierStatus;
        cAddress caPrimaryAddress;
        cSupplierCategory cscCategory;
        string sSupplierCode;
        double dAnnualTurnover;
        int? nCurrencyId;
        //cCurrency dTurnoverCurrency;
        string sInternalContact;
        int nNumberOfEmployees;
        DateTime? dtCreditRefChecked;
        string sWebURL;
        string sSupplierEmail;
        short nFinancialYearEnd;
        cFinancialStatus fsLastFinancialStatus;
        Dictionary<string, cSupplierContact> slContacts;
        SortedList<int, object> lstUserdefined;
        bool bIsSupplier;
        bool bIsReseller;

        public cSupplier(int sid, int subaccid, string name, cSupplierStatus status, cSupplierCategory category, string suppliercode, cAddress address, string weburl, short FYE, cFinancialStatus financialstatus, int? currencyId, double annualTurnover, int numberOfEmployees, Dictionary<string, cSupplierContact> contacts, SortedList<int, object> userfields, DateTime? creditrefchecked, string internalcontact, string email, bool issupplier, bool isreseller)
        {
            supplierid = sid;
            nSubAccountId = subaccid;
            suppliername = name;
            cssSupplierStatus = status;
            cscCategory = category;
            sSupplierCode = suppliercode;
            caPrimaryAddress = address;
            sWebURL = weburl;
            sSupplierEmail = email;
            nFinancialYearEnd = FYE;
            dtCreditRefChecked = creditrefchecked;
            fsLastFinancialStatus = financialstatus;
            nCurrencyId = currencyId;
            sInternalContact = internalcontact;
            slContacts = contacts;
            nNumberOfEmployees = numberOfEmployees;
            dAnnualTurnover = annualTurnover;
            lstUserdefined = userfields;
            bIsSupplier = issupplier;
            bIsReseller = isreseller;
        }

        #region properties
        public int SupplierId
        {
            get { return supplierid; }
        }
        public int subAccountId
        {
            get { return nSubAccountId; }
        }
        public string SupplierName
        {
            get { return suppliername; }
        }
        public cSupplierStatus SupplierStatus
        {
            get { return cssSupplierStatus; }
        }
        public cSupplierCategory SupplierCategory
        {
            get { return cscCategory; }
        }
        public string SupplierCode
        {
            get { return sSupplierCode; }
        }
        public cAddress PrimaryAddress
        {
            get { return caPrimaryAddress; }
        }
        public Dictionary<string, cSupplierContact> SupplierContacts
        {
            get { return slContacts; }
        }
        public string WebURL
        {
            get { return sWebURL; }
        }
        public string SupplierEmail
        {
            get { return sSupplierEmail; }
        }
        public DateTime? CreditRefChecked
        {
            get { return dtCreditRefChecked; }
        }
        public short FinancialYearEnd
        {
            get { return nFinancialYearEnd; }
        }
        public cFinancialStatus LastFinancialStatus
        {
            get { return fsLastFinancialStatus; }
        }
        public int? TurnoverCurrencyId
        {
            get { return nCurrencyId; }
        }
        public double AnnualTurnover
        {
            get { return dAnnualTurnover; }
        }
        public int NumberOfEmployees
        {
            get { return nNumberOfEmployees; }
        }
        public string InternalContact
        {
            get { return sInternalContact; }
        }
        public SortedList<int, object> userdefined
        {
            get { return lstUserdefined; }
        }
        public bool IsSupplier
        {
            get { return bIsSupplier; }
        }
        public bool IsReseller
        {
            get { return bIsReseller; }
        }
        #endregion
    }
}

