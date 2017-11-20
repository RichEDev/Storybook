namespace SpendManagementLibrary.Report
{
    using System;

    /// <summary>
    /// An instance of a Common Report Column.
    /// </summary>
    public class ReportCommonColumn
    {
        /// <summary>
        /// The Table Id of this Common Column.
        /// </summary>
        public Guid TableId { get; set; }

        /// <summary>
        /// The Field Id of this Common Column
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// The Join path (used in Reports to generate the tree) for this common column.
        /// </summary>
        public string JoinString { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="ReportCommonColumn"/>.  This is used to generate a view tree in reports for instance.
        /// </summary>
        /// <param name="tableId">The ID of the <see cref="cTable"/>this Common Column is for.</param>
        /// <param name="fieldId">The ID of the <see cref="cField"/>this common column relates to.</param>
        /// <param name="joinString">A string representation of the join between the base table and the common field.</param>
        public ReportCommonColumn(Guid tableId, Guid fieldId, string joinString)
        {
            this.TableId = tableId;
            this.FieldId = fieldId;
            this.JoinString = joinString;
        }
    }
}
