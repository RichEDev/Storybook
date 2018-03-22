namespace SpendManagementLibrary
{
    using System;
    using SpendManagementLibrary.Definitions.JoinVia;

    /// <summary>
    /// Sort columns for greenlight view
    /// </summary>
    public class GreenLightSortColumn
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="fieldID"></param>
        /// <param name="sortDirection"></param>
        /// <param name="joinVia"></param>
        public GreenLightSortColumn(Guid fieldID, SortDirection sortDirection, JoinVia joinVia)
        {
            this.FieldID = fieldID;
            this.SortDirection = sortDirection;
            this.JoinVia = joinVia;
        }

        #region properties
        /// <summary>
        /// The fieldid of the column to sort
        /// </summary>
        public Guid FieldID { get; set; }

        /// <summary>
        /// The direction of the sort whether it be ascending or descending
        /// </summary>
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// The JoinViaID for the column if one is present, otherwise 0
        /// </summary>
        public JoinVia JoinVia { get; set; }

        #endregion
    }
}