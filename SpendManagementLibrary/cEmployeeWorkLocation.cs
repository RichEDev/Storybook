using System;

namespace SpendManagementLibrary
{
    [Serializable]
    public class cEmployeeWorkLocation
    {
        private int nEmployeeID;
        private int nLocationID;
        private DateTime? dtStartDate;
        private DateTime? dtEndDate;
        private bool bActive;
        private bool bTemporary;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;
        private int? _esrLocationId;
        private bool rotational;
        private bool primaryRotational;

        public cEmployeeWorkLocation(int employeeWorkAddressId, int employeeid, int locationid, DateTime? startdate, DateTime? enddate, bool active, bool temporary, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, int? esrAssignmentLocationId, bool primaryRotational)
        {
            this.EmployeeWorkAddressId = employeeWorkAddressId;
            nEmployeeID = employeeid;
            nLocationID = locationid;
            dtStartDate = startdate;
            dtEndDate = enddate;
            bActive = active;
            bTemporary = temporary;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
            this.EsrAssignmentLocationId = esrAssignmentLocationId;
            this.rotational = false;
            this.primaryRotational = primaryRotational;
        }

        #region properties

        /// <summary>
        /// The primary key of the employee address
        /// </summary>
        public int EmployeeWorkAddressId { get; private set; }

        /// <summary>
        /// The ID of the employee this location belongs to
        /// </summary>
        public int EmployeeID
        {
            get { return nEmployeeID; }
        }
        /// <summary>
        /// The ID of the location
        /// </summary>
        public int LocationID
        {
            get { return nLocationID; }
        }
        /// <summary>
        /// The date the employee started working at this location
        /// </summary>
        public DateTime? StartDate
        {
            get { return dtStartDate; }
        }
        /// <summary>
        /// The date the employee stopped working at this location
        /// </summary>
        public DateTime? EndDate
        {
            get { return dtEndDate; }
        }
        /// <summary>
        /// Whether or not the employee still works at this location
        /// </summary>
        [Obsolete("No longer used")]
        public bool Active
        {
            get { return bActive; }
        }
        /// <summary>
        /// Whether or not the employee works at this location temporarily
        /// </summary>
        public bool Temporary
        {
            get { return bTemporary; }
        }
        /// <summary>
        /// The date and time this work location was created
        /// </summary>
        public DateTime CreatedOn
        {
            get { return dtCreatedOn; }
        }
        /// <summary>
        /// The employee this work location was created by
        /// </summary>
        public int CreatedBy
        {
            get { return nCreatedBy; }
        }
        /// <summary>
        /// The date and time this work locations was last modified
        /// </summary>
        public DateTime? ModifiedOn
        {
            get { return dtModifiedOn; }
        }
        /// <summary>
        /// The employee this work location was last modified by
        /// </summary>
        public int? ModifiedBy
        {
            get { return nModifiedBy; }
        }

        /// <summary>
        /// The ESR Assignment-Location ID
        /// </summary>
        public int? EsrAssignmentLocationId { get; private set; }

        public EsrAssignmentLocation GetEsrLocation(ICurrentUserBase currentUser)
        {
            return this.EsrAssignmentLocationId == null ? null
                : new EsrAssignmentLocations(currentUser.AccountID).GetEsrAssignmentLocationById((int)this.EsrAssignmentLocationId);
        }

        /// <summary>
        /// Get if this address is part of a rotation.
        /// </summary>
        public bool Rotational { get { return this.rotational; } }

        /// <summary>
        /// Get if this address is the primary rotation.
        /// </summary>
        public bool PrimaryRotational { get { return this.primaryRotational; } }

        #endregion

    }
}
