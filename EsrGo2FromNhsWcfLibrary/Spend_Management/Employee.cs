namespace EsrGo2FromNhsWcfLibrary.Spend_Management
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;

    /// <summary>
    /// The password method.
    /// </summary>
    public enum PwdMethod
    {
        /// <summary>
        /// The FW basic.
        /// </summary>
        FwBasic = 0,

        /// <summary>
        /// The hash.
        /// </summary>
        Hash = 1,

        /// <summary>
        /// The SHA hash.
        /// </summary>
        ShaHash = 2,

        /// <summary>
        /// The m d 5.
        /// </summary>
        Md5 = 3,

        /// <summary>
        /// The RIJNDAEL managed.
        /// </summary>
        RijndaelManaged = 4,

        /// <summary>
        /// The no crypt.
        /// </summary>
        NoCrypt = 99
    }

    /// <summary>
    /// The employee creation method.
    /// </summary>
    public enum EmployeeCreationMethod
    {
        /// <summary>
        /// Via add/edit expenses
        /// </summary>
        Manually = 0,

        /// <summary>
        /// Via the ESR Outbound import
        /// </summary>
        EsrOutbound,

        /// <summary>
        /// Via the Excel implementation import
        /// </summary>
        ImplementationImport,

        /// <summary>
        /// Via the data import wizard
        /// </summary>
        ImportWizard,

        /// <summary>
        /// Via Self Registration
        /// </summary>
        SelfRegistration
    }

    /// <summary>
    /// The employee Data Class.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Employee, TableId = "618DB425-F430-4660-9525-EBAB444ED754")]
    public class Employee : DataClassBase
    {
        /// <summary>
        /// The employee id.
        /// </summary>
        [DataClass(IsKeyField = true, FieldId = "EDA990E3-6B7E-4C26-8D38-AD1D77FB2FBF")]
        [DataMember(IsRequired = true, Order = 0)]
        public int employeeid = 0;
        [DataMember]
        [DataClass( FieldId = "1C45B860-DDAA-47DA-9EEC-981F59CCE795")]
        public string username = string.Empty;
        [DataMember]
        [DataClass(FieldId = "669236FD-CBA3-4E80-B58D-68A52C45032B")]
        public string password = string.Empty;
        [DataMember]
        [DataClass(FieldId = "28471060-247D-461C-ABF6-234BCB4698AA")]
        public string title = string.Empty;
        [DataMember]
        [DataClass(FieldId = "6614ACAD-0A43-4E30-90EC-84DE0792B1D6")]
        public string firstname = string.Empty;
        [DataMember]
        [DataClass(FieldId = "9D70D151-5905-4A67-944F-1AD6D22CD931")]
        public string surname = string.Empty;
        [DataMember]
        [DataClass(FieldId = "81D172A8-91F4-4BD8-A9F1-C846F284D3B8")]
        public int? mileagetotal = null;
        [DataMember]
        [DataClass(FieldId = "0F951C3E-29D1-49F0-AC13-4CFCABF21FDA")]
        public string email = string.Empty;
        [DataMember]
        [DataClass(FieldId = "30827090-7D91-4D2C-8E64-DDC77B57B5E3")]
        public int currefnum = 0;
        [DataMember]
        [DataClass(FieldId = "6DADBF48-DA43-45B5-892D-F3290B1F57A0")]
        public int curclaimno = 1;
        [DataMember]
        [DataClass(FieldId = "")]
        public int? speedo = null;
        [DataMember]
        [DataClass(FieldId = "6A76898B-4052-416C-B870-61479CA15AC1")]
        public string payroll = string.Empty;
        [DataMember]
        [DataClass(FieldId = "5F4A4551-1C05-4C85-B6D9-06D036BC327E")]
        public string position = string.Empty;
        [DataMember]
        [DataClass(FieldId = "D22C2D3F-7B33-4744-A1CF-C55296161CC3")]
        public string telno = string.Empty;
        [DataMember]
        [DataClass(FieldId = "25860CD6-8026-4F86-B5E7-5436D25AB244")]
        public string creditor = string.Empty;
        [DataMember]
        [DataClass(FieldId = "3A6A93F0-9B30-4CC2-AFC4-33EC108FA77A")]
        public bool archived = false;
        [DataMember]
        [DataClass(FieldId = "4F615406-8D1F-47B3-821B-88BADE48205E")]
        public int? groupid = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public int? roleid = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public string hint = string.Empty;
        [DataMember]
        [DataClass(FieldId = "")]
        public DateTime? lastchange = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public int additems = 0;
        [DataMember]
        [DataClass(FieldId = "")]
        public string cardnum = string.Empty;
        [DataMember]
        [DataClass(FieldId = "")]
        public bool userole = false;
        [DataMember]
        [DataClass(FieldId = "")]
        public int? costcodeid = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public int? departmentid = null;
        [DataMember]
        [DataClass(FieldId = "4F1C1895-A6B0-4EB3-A470-B34E43F07EF8")]
        public string extension = string.Empty;
        [DataMember]
        [DataClass(FieldId = "FB07BC07-897D-438C-818F-5F5F0A0B7769")]
        public string pagerno = string.Empty;
        [DataMember]
        [DataClass(FieldId = "0548653D-3255-4404-8BFB-20E5B83F7E45")]
        public string mobileno = string.Empty;
        [DataMember]
        [DataClass(FieldId = "79258E43-F986-4706-9289-18F8B9E43499")]
        public string faxno = string.Empty;
        [DataMember]
        [DataClass(FieldId = "CDE508F2-F2B2-439C-8A1C-B70EE65E2ABC")]
        public string homeemail = string.Empty;
        [DataMember]
        [DataClass(FieldId = "96F11C6D-7615-4ABD-94EC-0E4D34E187A0")]
        public int? linemanager = null;
        [DataMember]
        [DataClass(FieldId = "DAFF7A05-3489-4AF0-92C2-E7D4F98691CD")]
        public int? advancegroupid = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public int mileage = 0;
        [DataMember]
        [DataClass(FieldId = "")]
        public int mileageprev = 0;
        [DataMember]
        [DataClass(FieldId = "")]
        public bool customiseditems = false;
        [DataMember]
        [DataClass(FieldId = "B8D81E67-51C2-483A-8015-606A3DBDA0A4")]
        public bool active = false;
        [DataMember]
        [DataClass(FieldId = "031CF9C5-FFFF-4A7A-AB98-54617A91766A")]
        public int? primarycountry = null;
        [DataMember]
        [DataClass(FieldId = "026CC190-20D8-427E-9AE2-200747F45670")]
        public int? primarycurrency = null;
        [DataMember]
        [DataClass(FieldId = "EC8BC4EE-A235-4803-852E-3339926EB763")]
        public bool verified = false;
        [DataMember]
        [DataClass(FieldId = "04108CBF-50F4-409A-9047-92FC63E050E6")]
        public DateTime? licenceexpiry = null;
        [DataMember]
        [DataClass(FieldId = "3F457C3F-995C-4DA4-AD24-10301518AB1E")]
        public DateTime? licencelastchecked = null;
        [DataMember]
        [DataClass(FieldId = "FE106AA5-469F-488D-B9BC-B12CD06C1124")]
        public int? licencecheckedby = null;
        [DataMember]
        [DataClass(FieldId = "79B7287B-9D74-404F-941D-6C15FFC9EA71")]
        public string licencenumber;
        [DataMember]
        [DataClass(FieldId = "DD8B8DA9-57D9-449D-9FB1-57356B6D3E50")]
        public int? groupidcc = null;
        [DataMember]
        [DataClass(FieldId = "DA00F527-705C-4731-82B5-287BE3DF7E5B")]
        public int? groupidpc = null;
        [DataMember]
        [DataClass(FieldId = "F3F2E4BF-F594-4125-A924-FD6703DB2F2D")]
        public DateTime? CreatedOn = null;
        [DataMember]
        [DataClass(FieldId = "6292782E-2040-4A1E-A4E3-2FEA8C9D9A5D")]
        public int? CreatedBy = null;
        [DataMember]
        [DataClass(FieldId = "E27C6957-0435-4177-B1A6-B56459466C40")]
        public DateTime? ModifiedOn = null;
        [DataMember]
        [DataClass(FieldId = "3C749B9D-6E8F-4711-96DC-48C8AAF8ABC8")]
        public int? ModifiedBy = null;
        [DataMember]
        [DataClass(FieldId = "8816CAEC-B520-4223-B738-47D2F22F3E1A")]
        public string country = string.Empty;
        [DataMember]
        [DataClass(FieldId = "74391669-0070-4D19-AF73-3AB4EA4D55BB")]
        public string ninumber = string.Empty;
        [DataMember]
        [DataClass(FieldId = "075FDA2D-EFD4-461F-928F-9FF582A8B6AC")]
        public string maidenname = string.Empty;
        [DataMember]
        [DataClass(FieldId = "B3CAF703-E72B-4EB8-9D5C-B389E16C8C43")]
        public string middlenames = string.Empty;
        [DataMember]
        [DataClass(FieldId = "A4546216-B8EA-4DDF-B218-1A8C0493274C")]
        public string gender = string.Empty;
        [DataMember]
        [DataClass(FieldId = "486554C0-75ED-4718-BA32-3C11EAA5EE79")]
        public DateTime? dateofbirth = null;
        [DataMember]
        [DataClass(FieldId = "76473C0A-DF08-40F9-8DE0-632D0111A912")]
        public DateTime? hiredate = null;
        [DataMember]
        [DataClass(FieldId = "B7CBF994-4A23-4405-93DF-D66D4C3ED2A3")]
        public DateTime? terminationdate = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public int? homelocationid = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public int? officelocationid = null;
        [DataMember]
        [DataClass(FieldId = "A93ED9A2-6202-46F2-88DB-C0B5A6863DF5")]
        public string applicantnumber = string.Empty;
        [DataMember]
        [DataClass(FieldId = "8D8C73D2-FA4F-47ED-866F-2CEE890B80F9")]
        public bool applicantactivestatusflag = false;
        [DataMember]
        [DataClass(FieldId = "")]
        public PwdMethod? passwordMethod = null;
        [DataMember]
        [DataClass(FieldId = "DD45BD97-8CBB-43D2-92CC-4A1361D35885")]
        public string name = string.Empty;
        [DataMember]
        [DataClass(FieldId = "AC1A1733-BA2B-4A5E-8183-C853F1B139BF")]
        public string accountnumber = string.Empty;
        [DataMember]
        [DataClass(FieldId = "A1379097-387B-4537-BCE3-0A8C87A1F4F7")]
        public string accounttype = string.Empty;
        [DataMember]
        [DataClass(FieldId = "B4A0C1A9-1FA1-49F8-85FA-3E472E2CB141")]
        public string sortcode = string.Empty;
        [DataMember]
        [DataClass(FieldId = "7FB886BF-CF78-4962-9D0F-A957863B217C")]
        public string reference = string.Empty;
        [DataMember]
        [DataClass(FieldId = "")]
        public int? localeID = null;
        [DataMember]
        [DataClass(FieldId = "9573ED0B-814B-4CB0-916A-8CE25893617D")]
        public int? NHSTrustID = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public int logonCount = 0;
        [DataMember]
        [DataClass(FieldId = "")]
        public int retryCount = 0;
        [DataMember]
        [DataClass(FieldId = "")]
        public bool firstLogon = false;
        [DataMember]
        [DataClass(FieldId = "")]
        public int? licenceAttachID = null;
        [DataMember]
        [DataClass(FieldId = "25960ABD-3D30-4EB3-9F51-B553F6CDABC3")]
        public int? defaultSubAccountId = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public DateTime? CacheExpiry = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public int? supportPortalAccountID = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public string supportPortalPassword = string.Empty;
        [DataMember]
        [DataClass(FieldId = "473BF9A3-4D7D-4993-AAB4-096FEE8002B4")]
        public EmployeeCreationMethod CreationMethod;
        [DataMember]
        [DataClass(FieldId = "9DCC6C76-09DA-415A-8B15-7C7F7978BC30")]
        public DateTime? mileagetotaldate = null;
        [DataMember]
        [DataClass(FieldId = "")]
        public bool adminonly = false;
        [DataMember]
        [DataClass(FieldId = "EE09104F-BEAD-44DA-8B05-17A6CE6BDFC5")]
        public bool locked = false;
        [DataMember]
        [DataClass(FieldId = "")]
        public long? ESRPersonId = null;
        [DataMember]
        [DataClass(FieldId = "3309E5E0-6105-4E2C-BE90-A8EAD03EACF2")]
        public string preferredname = string.Empty;
        [DataMember]
        [DataClass(FieldId = "D7DA409B-3E6D-4C92-B737-ADB4D90C23E5")]
        public string employeenumber = string.Empty;
        [DataMember]
        [DataClass(FieldId = "1883FF7E-12CE-4E81-A1A1-FFF6142FF13F")]
        public string esrpersontype = string.Empty;
        [DataMember]
        [DataClass(FieldId = "EA2ECF94-AB38-475F-8454-C57DA22C88BA")]
        public string nhsuniqueid = string.Empty;
        [DataMember]
        [DataClass(FieldId = "7626D350-D51A-42EE-84C9-127FB1EDC580")]
        public DateTime? esreffectivestartdate = null;
        [DataMember]
        [DataClass(FieldId = "02B90A6A-D1F0-4B99-A3A4-06C6BAE02560")]
        public DateTime? esreffectiveenddate = null;
    }
}