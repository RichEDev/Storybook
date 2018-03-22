namespace SpendManagementLibrary
{
    using System.Collections.Generic;

    /// <summary>
    /// Store the custom entity form section and its associated fields
    /// </summary>
    public struct sCEFormSection
    {
        public string SectionName;
        public string SectionControlName;
        public List<sCEFieldDetails> Fields;
        public byte Order;
    }
}