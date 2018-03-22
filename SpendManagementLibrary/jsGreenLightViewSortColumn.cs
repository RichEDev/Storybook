namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// The information required for a JS view sort column
    /// </summary>
    public class jsGreenLightViewSortColumn
    {
        public string SortID = string.Empty;
        public Guid FieldID = Guid.Empty;
        public int JoinViaID = 0;
        public string JoinViaPath = string.Empty;
        public string JoinViaCrumbs = string.Empty;
    }
}