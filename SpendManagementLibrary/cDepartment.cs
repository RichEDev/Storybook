using System;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cDepartment
    {
        public int DepartmentId { get; set; }
        public string Department { get; private set; }
        public readonly string Description;
        public bool Archived { get; set; }
        public DateTime CreatedOn { get; private set; }
        public int CreatedBy { get; private set; }
        public DateTime? ModifiedOn { get; private set; }
        public int? ModifiedBy { get; private set; }
        public SortedList<int, object> UserdefinedFields;

        public cDepartment(int departmentid, string department, string description, bool archived, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, object> userdefined)
        {
            this.DepartmentId = departmentid;
            this.Department = department;
            this.Description = description;
            this.Archived = archived;
            this.CreatedOn = createdon;
            this.CreatedBy = createdby;
            this.ModifiedOn = modifiedon;
            this.ModifiedBy = modifiedby;
            this.UserdefinedFields = userdefined;
        }
    }
}
