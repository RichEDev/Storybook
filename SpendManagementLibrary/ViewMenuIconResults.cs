namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// View Menu Icon results struct
    /// </summary>
    [Serializable]
    public struct ViewMenuIconResults
    {
        /// <summary>
        /// Actual menu icons
        /// </summary>
        public List<ViewMenuIcon> MenuIcons;

        /// <summary>
        /// The result number to start from
        /// </summary>
        public int ResultStartNumber;

        /// <summary>
        /// The result number we ended on
        /// </summary>
        public int ResultEndNumber;

        /// <summary>
        /// Indicates if there are further results to be returned
        /// </summary>
        public bool FurtherResults;
    }
}