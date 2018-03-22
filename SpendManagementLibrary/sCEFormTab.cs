namespace SpendManagementLibrary
{
    using System.Collections.Generic;

    /// <summary>
    /// Store the custom entity form tab and its associated sections
    /// </summary>
    public struct sCEFormTab
    {
        public string TabName;
        public string TabControlName;
        public List<sCEFormSection> Sections;
        public byte Order;
    }
}