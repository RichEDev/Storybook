namespace SpendManagementLibrary.Report
{
    using System;
    /// <summary>
    /// A selected criteria from the report add/edit page
    /// </summary>
    public class SelectedCriteria
    {
        /// <summary>
        /// The Guid of the field selected.
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// The ID of the field selected.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The condition 
        /// </summary>
        public ConditionType Condition { get; set; }

        /// <summary>
        /// The connector type (and/or)
        /// </summary>
        public ConditionJoiner Joiner { get; set; }

        /// <summary>
        /// The value to compare against
        /// </summary>
        public object[] Value1 { get; set; }

        /// <summary>
        /// The second value if Between is selected.
        /// </summary>
        public object[] Value2 { get; set; }

        /// <summary>
        /// The order of this criteria.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// If entered at runtime.
        /// </summary>
        public bool RunTime { get; set; }

        /// <summary>
        /// The group ID
        /// </summary>
        public int Group { get; set; }

        /// <summary>
        /// If this is a drill down criteria
        /// </summary>
        public bool DrillDown { get; set; }

        /// <summary>
        /// The join via ID
        /// </summary>
        public int JoinViaId { get; set; }

        /// <summary>
        /// A string representation of the query tree to get to this field.
        /// </summary>
        public string Crumbs { get; set; }
    }
}
