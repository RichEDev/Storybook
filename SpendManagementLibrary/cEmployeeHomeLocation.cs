namespace SpendManagementLibrary
{
    using System;

    using SpendManagementLibrary.Employees;

    [Serializable]
    public class cEmployeeHomeLocation
    {
        

        #region Fields

        private readonly DateTime dtCreatedOn;

        private readonly DateTime? dtEndDate;

        private readonly DateTime? dtModifiedOn;

        private readonly DateTime? dtStartDate;

        private readonly int nCreatedBy;

        private readonly int nEmployeeID;

        private readonly int nEmployeeLocationID;

        private readonly int nLocationID;

        private readonly int? nModifiedBy;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Instantiate a new instance of <see cref="cEmployeeHomeLocation"/>
        /// </summary>
        /// <param name="employeelocationid">The unique ID of the location</param>
        /// <param name="employeeid">The Id of the <see cref="Employee"/>record</param>
        /// <param name="locationid">The unique ID of the <see cref="cAddress"/>record</param>
        /// <param name="startdate">The date the address is active</param>
        /// <param name="enddate">The date (if any) that the address is inactive</param>
        /// <param name="createdon">The date created</param>
        /// <param name="createdby">The <see cref="Employee"/>ID of the user who created the address</param>
        /// <param name="modifiedon">The date the record was modified </param>
        /// <param name="modifiedby">The <see cref="Employee"/>ID of the user who modified the address</param>
        /// <param name="esrAddressId">The ID of the "ESRAddress" record</param>
        public cEmployeeHomeLocation(int employeelocationid, int employeeid, int locationid, DateTime? startdate, DateTime? enddate, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, long? esrAddressId = null)
        {
            this.EsrAddressId = esrAddressId;
            this.nEmployeeLocationID = employeelocationid;
            this.nEmployeeID = employeeid;
            this.nLocationID = locationid;
            this.dtStartDate = startdate;
            this.dtEndDate = enddate;
            this.dtCreatedOn = createdon;
            this.nCreatedBy = createdby;
            this.dtModifiedOn = modifiedon;
            this.nModifiedBy = modifiedby;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The employee this work location was created by
        /// </summary>
        public int CreatedBy
        {
            get { return this.nCreatedBy; }
        }

        /// <summary>
        /// The date and time this work location was created
        /// </summary>
        public DateTime CreatedOn
        {
            get { return this.dtCreatedOn; }
        }

        /// <summary>
        /// The ID of the employee this location belongs to
        /// </summary>
        public int EmployeeID
        {
            get { return this.nEmployeeID; }
        }

        /// <summary>
        /// The primary key of the employee location
        /// </summary>
        public int EmployeeLocationID
        {
            get { return this.nEmployeeLocationID; }
        }

        /// <summary>
        /// The date the employee stopped working at this location
        /// </summary>
        public DateTime? EndDate
        {
            get { return this.dtEndDate; }
        }

        /// <summary>
        /// The ID of the location
        /// </summary>
        public int LocationID
        {
            get { return this.nLocationID; }
        }

        /// <summary>
        /// The employee this work location was last modified by
        /// </summary>
        public int? ModifiedBy
        {
            get { return this.nModifiedBy; }
        }

        /// <summary>
        /// The date and time this work locations was last modified
        /// </summary>
        public DateTime? ModifiedOn
        {
            get { return this.dtModifiedOn; }
        }

        /// <summary>
        /// The date the employee started working at this location
        /// </summary>
        public DateTime? StartDate
        {
            get { return this.dtStartDate; }
        }

        /// <summary>
        /// The ID of the "ESRAddress" record related to this home address
        /// </summary>
        public long? EsrAddressId { get;}
        #endregion
    }
}
