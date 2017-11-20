using System;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    //[Serializable()]
    //public class cCompany : System.Collections.IComparer
    //{
    //    protected int nCompanyid;
    //    protected string sCompany;
    //    private string sPostcode;
    //    private bool bIsPrivate;
    //    private int nCreatedby;
    //    private bool bArchived;

    //    public cCompany(int companyid, string company, string postcode, bool isPrivate, int createdby, bool archived)
    //    {
    //        nCompanyid = companyid;
    //        sCompany = company;
    //        sPostcode = postcode;
    //        bIsPrivate = isPrivate;
    //        nCreatedby = createdby;
    //        bArchived = archived;
    //    }

    //    /// <summary>
    //    /// Parameterless constructor for serializing over web methods
    //    /// </summary>
    //    public cCompany()
    //    {
    //    }

    //    public int Compare(object obj1, object obj2)
    //    {
    //        return -1;
    //    }

    //    public void updateID(int id)
    //    {
    //        nCompanyid = id;
    //    }

    //    public enum AddressCreationMethod 
    //    {
    //        ClaimantAdded=1,
    //        AdministratorAdded=2,
    //        ESROutboundAdded=3,
    //        DataImportWizard=4,
    //        ImplementationImportRoutine=5
    //    }

    //    #region properties
    //    public int companyid
    //    {
    //        get { return nCompanyid; }
    //        set { nCompanyid = value; }
    //    }
    //    public string company
    //    {
    //        get { return sCompany; }
    //        set { sCompany = value; }

    //    }
    //    public string postcode
    //    {
    //        get { return sPostcode; }
    //        set { sPostcode = value; }
    //    }

    //    public int createdby
    //    {
    //        get { return nCreatedby; }
    //        set { nCreatedby = value; }
    //    }
    //    /// <summary>
    //    /// Flag to set a location as private 
    //    /// </summary>
    //    public bool isPrivate
    //    {
    //        get { return bIsPrivate; }
    //        set { bIsPrivate = value; }
    //    }
    //    public bool archived
    //    {
    //        get { return bArchived; }
    //        set { bArchived = value; }
    //    }

    //    #endregion
    //}

    //public class CompanyWithAddress : cCompany
    //{
    //    private string sAddress1;
    //    private string sAddress2;
    //    private string sCity;
    //    private string sCounty;
    //    private int nCountry;

    //    public CompanyWithAddress(int companyid, string company, string address1, string address2, string city, string county, string postcode, int country, bool isPrivate, int createdby, bool archived)
    //        : base(companyid, company, postcode, isPrivate, createdby, archived)
    //    {
    //        sAddress1 = address1;
    //        sAddress2 = address2;
    //        sCity = city;
    //        sCounty = county;

    //        nCountry = country;
    //    }

    //    #region properties
    //    public string address1
    //    {
    //        get { return sAddress1; }
    //        set { sAddress1 = value; }
    //    }
    //    public string address2
    //    {
    //        get { return sAddress2; }
    //        set { sAddress2 = value; }
    //    }
    //    public string city
    //    {
    //        get { return sCity; }
    //        set { sCity = value; }
    //    }
    //    public string county
    //    {
    //        get { return sCounty; }
    //        set { sCounty = value; }
    //    }

    //    public int country
    //    {
    //        get { return nCountry; }
    //        set { nCountry = value; }
    //    }
    //    #endregion
    //}
    //public class FullCompany : CompanyWithAddress
    //{
    //    private string sCompanycode;
        
    //    private string sComment;
    //    private bool bShowto;
    //    private bool bShowfrom;
    //    private DateTime dtCreatedon;
        
    //    private DateTime dtModifiedon;
    //    private int nModifiedby;
       
    //    private int nParentCompanyID;
    //    private bool bIsCompany;
    //    SortedList<int, object> lstUserdefined;
        
    //    private AddressCreationMethod eCreationMethod;

    //    public FullCompany(int companyid, string company, string companycode, bool archived, string comment, bool showto, bool showfrom, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby, string address1, string address2, string city, string county, string postcode, int country, int parentcompanyid, bool iscompany, SortedList<int, object> userdefined, bool isPrivate, AddressCreationMethod creationMethod) : base (companyid, company, address1, address2, city, county, postcode,country, isPrivate, createdby, archived)
    //    {
    //        sCompanycode = companycode;
            
    //        sComment = comment;
    //        bShowto = showto;
    //        bShowfrom = showfrom;
    //        dtCreatedon = createdon;
            
    //        dtModifiedon = modifiedon;
    //        nModifiedby = modifiedby;
            
    //        nParentCompanyID = parentcompanyid;
    //        bIsCompany = iscompany;
    //        lstUserdefined = userdefined;
            
    //        eCreationMethod = creationMethod;
    //    }

    //    #region properties
    //    public string companycode
    //    {
    //        get { return sCompanycode; }
    //        set { sCompanycode = value; }
    //    }
        
    //    public string comment
    //    {
    //        get { return sComment; }
    //        set { sComment = value; }
    //    }
    //    public bool showto
    //    {
    //        get { return bShowto; }
    //        set { bShowto = value; }
    //    }
    //    public bool showfrom
    //    {
    //        get { return bShowfrom; }
    //        set { bShowfrom = value; }
    //    }
    //    public DateTime createdon
    //    {
    //        get { return dtCreatedon; }
    //        set { dtCreatedon = value; }
    //    }
        
    //    public DateTime modifiedon
    //    {
    //        get { return dtModifiedon; }
    //        set { dtModifiedon = value; }
    //    }
    //    public int modifiedby
    //    {
    //        get { return nModifiedby; }
    //        set { nModifiedby = value; }
    //    }
        
    //    public int parentcompanyid
    //    {
    //        get { return nParentCompanyID; }
    //        set { nParentCompanyID = value; }
    //    }
    //    public bool iscompany
    //    {
    //        get { return bIsCompany; }
    //        set { bIsCompany = value; }
    //    }

    //    public SortedList<int, object> userdefined
    //    {
    //        get { return lstUserdefined; }
    //        set { lstUserdefined = value; }
    //    }

        

    //    /// <summary>
    //    /// Who created this address
    //    /// </summary>
    //    public AddressCreationMethod CreationMethod
    //    {
    //        get { return eCreationMethod; }
    //        set { eCreationMethod = value; }
    //    }
    //    //public Dictionary<int, object> userdefined
    //    //{
    //    //    get { return lstUserdefined; }
    //    //    set { lstUserdefined = value; }
    //    //}
    //    #endregion
    //}
    //[Serializable()]
    //public struct sOnlineCompInfo
    //{
    //    public Dictionary<int, cCompany> lstonlinecomps;
    //    public List<int> lstcompids;
    //}

    //[Serializable()]
    //public class cCompanyDistance
    //{
    //    private int nLocationa;
    //    private int nLocationb;
    //    private decimal nDistance;
    //    private DateTime dtCreatedon;
    //    private int nCreatedby;

    //    public cCompanyDistance(int locationa, int locationb, decimal distance, DateTime createdon, int createdby)
    //    {
    //        nLocationa = locationa;
    //        nLocationb = locationb;
    //        nDistance = distance;
    //        dtCreatedon = createdon;
    //        nCreatedby = createdby;
    //    }

    //    #region properties
        
    //    public int locationa
    //    {
    //        get { return nLocationa; }
    //    }
    //    public int locationb
    //    {
    //        get { return nLocationb; }
    //    }
    //    public decimal distance
    //    {
    //        get { return nDistance; }
    //    }
    //    public DateTime createdon
    //    {
    //        get { return dtCreatedon; }
    //    }
    //    public int createdby
    //    {
    //        get { return nCreatedby; }
    //    }
    //    #endregion
    //}
}
