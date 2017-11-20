namespace SpendManagementApi.Models.Types
{
    using Interfaces;
    using System;
    using System.Runtime.Serialization;

    public class DeletableBaseExternalType : BaseExternalType, ISupportsDelete
    {
        /// <summary>
        /// Gets or sets a value indicating whether the element can be deleted
        /// </summary>
        [IgnoreDataMember]
        public bool ForDelete { get; set; }
    }


    /// <summary>
    /// Base External Type
    /// </summary>
    public class BaseExternalType
    {
        /// <summary>
        /// Gets or sets Employee Id of Creator
        /// </summary>
        [IgnoreDataMember]
        public int CreatedById { get; internal set; }

        /// <summary>
        /// Gets or sets Date and time on creation
        /// </summary>
        [IgnoreDataMember]
        public DateTime CreatedOn { get; internal set; }

        /// <summary>
        /// Gets or sets Employee Id of Modifier
        /// </summary>
        [IgnoreDataMember]
        public int? ModifiedById { get; internal set; }

        /// <summary>
        /// Gets or sets Date and Time on modification
        /// </summary>
        [IgnoreDataMember]
        public DateTime? ModifiedOn { get; internal set; }
        
        /// <summary>
        /// Gets or sets Account Id
        /// </summary>
        [IgnoreDataMember]
        public int? AccountId { get; set; }

        /// <summary>
        /// Gets or sets Employee Id
        /// </summary>
        [IgnoreDataMember]
        public int? EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets EsrAssignmentLocationId
        /// </summary>
        [IgnoreDataMember]
        public int? EsrAssignmentLocationId { get; set; }

        protected internal bool DateCompare(DateTime? first, DateTime? second)
        {
            return ((first.HasValue && second.HasValue)
                        ? first.Value.Subtract(second.Value).Days == 0
                        : (!first.HasValue && !second.HasValue));
        }
    }


    /// <summary>
    /// The same as BaseExternalType, but with added Archivability.
    /// </summary>
    public class ArchivableBaseExternalType : BaseExternalType, IArchivable
    {
        /// <summary>
        /// Whether this object is in an archived status.
        /// </summary>
        public bool Archived { get; set; }
    }

}