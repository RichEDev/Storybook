
namespace ApiCrud.DataClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// The employee Data Class.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class Employee : DataClassBase
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Employee"/> class.
        /// </summary>
        public Employee()
        {}

        /// <summary>
        /// Initialises a new instance of the <see cref="Employee"/> class.
        /// </summary>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="passwordmethod">
        /// The passwordmethod.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="firstname">
        /// The firstname.
        /// </param>
        /// <param name="surname">
        /// The surname.
        /// </param>
        /// <param name="startmileagetotal">
        /// The startmileagetotal.
        /// </param>
        /// <param name="startmileagetotaldate">
        /// The startmileagetotaldate.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="currefnum">
        /// The currefnum.
        /// </param>
        /// <param name="curclaimno">
        /// The curclaimno.
        /// </param>
        /// <param name="address1">
        /// The address 1.
        /// </param>
        /// <param name="address2">
        /// The address 2.
        /// </param>
        /// <param name="city">
        /// The city.
        /// </param>
        /// <param name="county">
        /// The county.
        /// </param>
        /// <param name="postcode">
        /// The postcode.
        /// </param>
        /// <param name="payroll">
        /// The payroll.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="telno">
        /// The telno.
        /// </param>
        /// <param name="creditor">
        /// The creditor.
        /// </param>
        /// <param name="archived">
        /// The archived.
        /// </param>
        /// <param name="groupid">
        /// The groupid.
        /// </param>
        /// <param name="lastchange">
        /// The lastchange.
        /// </param>
        /// <param name="faxno">
        /// The faxno.
        /// </param>
        /// <param name="homeemail">
        /// The homeemail.
        /// </param>
        /// <param name="extension">
        /// The extension.
        /// </param>
        /// <param name="pagerno">
        /// The pagerno.
        /// </param>
        /// <param name="mobileno">
        /// The mobileno.
        /// </param>
        /// <param name="linemanager">
        /// The linemanager.
        /// </param>
        /// <param name="advancegroup">
        /// The advancegroup.
        /// </param>
        /// <param name="primarycountry">
        /// The primarycountry.
        /// </param>
        /// <param name="primarycurrency">
        /// The primarycurrency.
        /// </param>
        /// <param name="verified">
        /// The verified.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <param name="licenceexpirydate">
        /// The licenceexpirydate.
        /// </param>
        /// <param name="licencelastchecked">
        /// The licencelastchecked.
        /// </param>
        /// <param name="licencecheckedby">
        /// The licencecheckedby.
        /// </param>
        /// <param name="licencenumber">
        /// The licencenumber.
        /// </param>
        /// <param name="groupidcc">
        /// The groupidcc.
        /// </param>
        /// <param name="groupidpc">
        /// The groupidpc.
        /// </param>
        /// <param name="ninumber">
        /// The ninumber.
        /// </param>
        /// <param name="middlenames">
        /// The middlenames.
        /// </param>
        /// <param name="maidenname">
        /// The maidenname.
        /// </param>
        /// <param name="gender">
        /// The gender.
        /// </param>
        /// <param name="dateofbirth">
        /// The dateofbirth.
        /// </param>
        /// <param name="hiredate">
        /// The hiredate.
        /// </param>
        /// <param name="terminationdate">
        /// The terminationdate.
        /// </param>
        /// <param name="country">
        /// The country.
        /// </param>
        /// <param name="createdon">
        /// The createdon.
        /// </param>
        /// <param name="createdby">
        /// The createdby.
        /// </param>
        /// <param name="modifiedon">
        /// The modifiedon.
        /// </param>
        /// <param name="modifiedby">
        /// The modifiedby.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="accountnumber">
        /// The accountnumber.
        /// </param>
        /// <param name="accounttype">
        /// The accounttype.
        /// </param>
        /// <param name="sortcode">
        /// The sortcode.
        /// </param>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <param name="localeID">
        /// The locale id.
        /// </param>
        /// <param name="NHSTrustID">
        /// The nhs trust id.
        /// </param>
        /// <param name="logoncount">
        /// The logon count.
        /// </param>
        /// <param name="retrycount">
        /// The retry count.
        /// </param>
        /// <param name="firstLogon">
        /// The first logon.
        /// </param>
        /// <param name="LicenceAttachID">
        /// The licence attach id.
        /// </param>
        /// <param name="defaultSubAccountId">
        /// The default sub account id.
        /// </param>
        /// <param name="empCreationMethod">
        /// The emp creation method.
        /// </param>
        /// <param name="adminonly">
        /// The adminonly.
        /// </param>
        /// <param name="locked">
        /// The locked.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        public Employee(int employeeid, string username, string password, int passwordmethod, string title, string firstname, string surname, int startmileagetotal, DateTime? startmileagetotaldate, string email, int currefnum, int curclaimno, string address1, string address2, string city, string county, string postcode, string payroll, string position, string telno, string creditor, bool archived, int groupid, DateTime lastchange,int additems, string faxno, string homeemail, string extension, string pagerno, string mobileno, int linemanager, int advancegroup, int primarycountry, int primarycurrency, bool verified, bool active, DateTime? licenceexpirydate, DateTime? licencelastchecked, int licencecheckedby, string licencenumber, int groupidcc, int groupidpc, string ninumber, string middlenames, string maidenname, string gender, DateTime? dateofbirth, DateTime? hiredate, DateTime? terminationdate, string country, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, string name, string accountnumber, string accounttype, string sortcode, string reference, int? localeID, int? NHSTrustID, int logoncount, int retrycount, bool firstLogon, int LicenceAttachID, int defaultSubAccountId, int empCreationMethod, bool adminonly, bool locked, string url)
        {
            this.EmployeeId = employeeid;
            this.username = username;
            this.password = password;
            this.passwordMethod = passwordmethod;
            this.title = title;
            this.firstname = firstname;
            this.surname = surname;
            this.mileagetotal = startmileagetotal;
            this.mileagetotaldate = startmileagetotaldate;
            this.email = email;
            this.currefnum = currefnum;
            this.curclaimno = curclaimno;
            this.address1 = address1;
            this.address2 = address2;
            this.city = city;
            this.county = county;
            this.postcode = postcode;
            this.payroll = payroll;
            this.position = position;
            this.telno = telno;
            this.creditor = creditor;
            this.archived = archived;
            this.groupid = groupid;
            this.lastchange = lastchange;
            this.additems = additems;

            this.ninumber = ninumber;
            this.terminationdate = terminationdate; // TODO:  left date?

            this.extension = extension;
            this.mobileno = mobileno;
            this.pagerno = pagerno;
            this.faxno = faxno;
            this.homeemail = homeemail;
            this.linemanager = linemanager;
            this.advancegroupid = advancegroup;

            this.primarycountry = primarycountry;
            this.primarycurrency = primarycurrency;

            this.verified = verified;
            this.active = active;
            this.licenceexpiry = licenceexpirydate;
            this.licencelastchecked = licencelastchecked;
            this.licencecheckedby = licencecheckedby;
            this.licencenumber = licencenumber;
            this.groupidcc = groupidcc;
            this.groupidpc = groupidpc;
            this.middlenames = middlenames;
            this.maidenname = maidenname;
            this.gender = gender;
            this.dateofbirth = dateofbirth;
            this.hiredate = hiredate;
            this.terminationdate = terminationdate;
            this.country = country;
            this.CreatedOn = createdon;
            this.CreatedBy = createdby;
            this.ModifiedOn = modifiedon;
            this.ModifiedBy = modifiedby;
            this.name = name;
            this.accountnumber = accountnumber;
            this.accounttype = accounttype;
            this.sortcode = sortcode;
            this.reference = reference;
            this.logonCount = logoncount;
            this.retryCount = retrycount;
            this.NHSTrustID = NHSTrustID;
            this.localeID = localeID;
            this.firstLogon = firstLogon;
            this.licenceAttachID = LicenceAttachID;
            this.defaultSubAccountId = defaultSubAccountId;
            this.CreationMethod = empCreationMethod;
            this.adminonly = adminonly;
            this.locked = locked;
            this.ActionResult = ApiResult.Success;
            this.Uri = url;
        }

        /// <summary>
        /// The employee id.
        /// </summary>
        [DataMember(IsRequired = true, Order = 0)]
        public int EmployeeId;

        [DataMember]
        public string username;

        [DataMember]
        public string password;

        [DataMember]
        public string title;

        [DataMember]
        public string firstname;

        [DataMember]
        public string surname;

        [DataMember]
        public int mileagetotal;

        [DataMember]
        public string email;

        [DataMember]
        public int currefnum;

        [DataMember]
        public int curclaimno;

        [DataMember]
        public int speedo;

        [DataMember]
        public string address1;

        [DataMember]
        public string address2;

        [DataMember]
        public string city;

        [DataMember]
        public string county;

        [DataMember]
        public string postcode;

        [DataMember]
        public string payroll;

        [DataMember]
        public string position;

        [DataMember]
        public string telno;

        [DataMember]
        public string creditor;

        [DataMember]
        public bool archived;

        [DataMember]
        public int groupid;

        [DataMember]
        public DateTime lastchange;

        [DataMember]
        public int additems;

        [DataMember]
        public int costcodeid;

        [DataMember]
        public int departmentid;

        [DataMember]
        public string extension;

        [DataMember]
        public string pagerno;

        [DataMember]
        public string mobileno;

        [DataMember]
        public string faxno;

        [DataMember]
        public string homeemail;

        [DataMember]
        public int linemanager;

        [DataMember]
        public int advancegroupid;

        [DataMember]
        public int mileage;

        [DataMember]
        public int mileageprev;

        [DataMember]
        public bool active;

        [DataMember]
        public int primarycountry;

        [DataMember]
        public int primarycurrency;

        [DataMember]
        public bool verified;

        [DataMember]
        public DateTime? licenceexpiry;

        [DataMember]
        public DateTime? licencelastchecked;

        [DataMember]
        public int licencecheckedby;

        [DataMember]
        public string licencenumber;

        [DataMember]
        public int groupidcc;

        [DataMember]
        public int groupidpc;

        [DataMember]
        public DateTime CreatedOn;

        [DataMember]
        public int CreatedBy;

        [DataMember]
        public DateTime? ModifiedOn;

        [DataMember]
        public int? ModifiedBy;

        [DataMember]
        public string country;

        [DataMember]
        public string ninumber;

        [DataMember]
        public string maidenname;

        [DataMember]
        public string middlenames;

        [DataMember]
        public string gender;

        [DataMember]
        public DateTime? dateofbirth;

        [DataMember]
        public DateTime? hiredate;

        [DataMember]
        public DateTime? terminationdate;

        [DataMember]
        public int homelocationid;

        [DataMember]
        public int officelocationid;

        [DataMember]
        public int passwordMethod;

        [DataMember]
        public string name;

        [DataMember]
        public string accountnumber;

        [DataMember]
        public string accounttype;

        [DataMember]
        public string sortcode;

        [DataMember]
        public string reference;

        [DataMember]
        public int? localeID;

        [DataMember]
        public int? NHSTrustID;

        [DataMember]
        public int logonCount;

        [DataMember]
        public int retryCount;

        [DataMember]
        public bool firstLogon;

        [DataMember]
        public int licenceAttachID;

        [DataMember]
        public int defaultSubAccountId;

        [DataMember]
        public DateTime CacheExpiry;

        [DataMember]
        public int CreationMethod;

        [DataMember]
        public DateTime? mileagetotaldate;

        [DataMember]
        public bool adminonly;

        [DataMember]
        public bool locked;

    }
}