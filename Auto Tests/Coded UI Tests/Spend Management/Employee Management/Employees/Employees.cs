
namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.Cars;

    public class ExpenseUser
    {
        private const string codedUIdentifier = "Custom Emp ";

        private Guid uniqueID; 
        public string UserName { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }

        public ExpenseUser()
            : this("Mr", string.Empty, string.Empty, string.Empty)
        { }

        public ExpenseUser(string title, string userName, string firstName, string surname)
        {
            uniqueID = Guid.NewGuid();
            UserName = codedUIdentifier + uniqueID;
            FirstName = firstName + uniqueID.ToByteArray()[0];
            Surname = surname + uniqueID.ToByteArray()[0];
            Title = title;
        }
    }

    #region Employee object
    public class Employees : ExpenseUser
    {
        /// <summary>
        /// The sql items
        /// </summary>
        public static string SqlItems = "Select * from employees where employeeid = @employeeid";

        public int employeeID { get; set; }
        public string middleName { get; set; }
        public string maidenName { get; set; }
        public string preferredName { get; set;}
        public string emailAddress { get; set; }
        public string extensionNumber { get; set; }
        public string mobileNumber { get; set; }
        public string PagerNumber { get; set; }
        public int? localeID { get; set; }

        public string creditAccount { get; set; }
        public string payrollNumber { get; set; }
        public string position { get; set; }
        public string nationalInsuranceNumner { get; set; }
        public DateTime? hireDate { get; set; }
        public DateTime? terminationDate { get; set; }
        public string employeeNumner { get; set; }
        public int? primaryCountry { get; set; }
        public int? primaryCurrency { get; set; }
        public int? lineManager { get; set; }
        public int startingMileage { get; set; }
        public DateTime? startingMileageDate { get; set; }
        public int currentMileage { get; set; }
        public int? trustID { get; set; }
        public string nhsUniqueID { get; set; }

        public string pEmailAddress { get; set; }
        public string telephoneNumber { get; set; }
        public string faxNumber { get; set; }
        public string licenceNumber { get; set; }
        public DateTime? licenceExpiryDate { get; set; }
        public DateTime? lastChecked { get; set; }
        public int checkedBy { get; set; }
        public string accountHolderName { get; set; }
        public string accountNumber { get; set; }
        public string accountType { get; set; }
        public string sortCode { get; set; }
        public string accountReference { get; set; }
        public string gender { get; set; }
        public DateTime? dateOfBirth { get; set; }

        public int? signoffGroup { get; set; }
        public int? ccsignoffGroup { get; set; }
        public int? pcsignoffGroup { get; set; }
        public int? asignoffGroup { get; set; }

        public bool stanNotify { get; set; }
        public bool esrOutImportSummary { get; set; }
        public bool esrOutImportStarted { get; set; }
        public bool esrOutManagerAccessUpdate { get; set; }
        public bool esrOutInvalidPost { get; set; }

        public int createdBy { get; set; }
        public int modifiedBy { get; set; }
        public bool active { get; set; }

        public List<Cars.Cars> employeeCars{get; set;}

        public List<ESRAssignments.ESRAssignments> employeeAssignments { get; set; }

        public List<string> EmployeeGridValues
        {
            get
            {
                return new List<string> { this.UserName, this.Title, this.FirstName, this.Surname, " " };
            }
        }

        public Employees()
            : base()
        { }

        public Employees(string userName, string title, string firstName, string surname, int employeeId = 0)
            : base(title, userName, firstName, surname)
        {
            employeeID = employeeId;
        }

        public string EmployeeFullName
        {
            get
            {
                return String.Concat(this.FirstName, " ", this.Surname);
            }
        }

        public string ApproverFullName
        {
            get
            {
                return String.Concat(this.FirstName, " ", this.Surname, " (Employee)");
            }
        }
    }
    #endregion

    /// <summary>
    /// The employee tab name.
    /// </summary>
    public enum EmployeeTabName
    {
        /// <summary>
        /// all tabs.
        /// </summary>
        [Description("AllTabs")]
        AllTabs = 0,

        /// <summary>
        /// The general details.
        /// </summary>
        [Description("General Details")]
        GeneralDetails = 1,

        /// <summary>
        /// The permission.
        /// </summary>
        [Description("Permission")]
        Permission = 2,

        /// <summary>
        /// The work.
        /// </summary>
        [Description("Work")]
        Work = 3,

        /// <summary>
        /// The personal.
        /// </summary>
        [Description("Personal")]
        Personal = 4,

        /// <summary>
        /// The claims.
        /// </summary>
        [Description("Claims")]
        Claims = 5,

        /// <summary>
        /// The notification.
        /// </summary>
        [Description("Notification")]
        Notification = 6
    }
}
