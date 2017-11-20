namespace SpendManagementLibrary
{
    /// <summary>
    /// A predefined section.
    /// </summary>
    public class TorchPredefinedSection
    {
        /// <summary>
        /// the ID of the predefined section.
        /// </summary>
        public int SectionId { get; set; }
        /// <summary>
        /// The matching <TorchProject></TorchProject> ID.
        /// </summary>
        public int MergeProjectId { get; set; }
        /// <summary>
        /// The matching <TorchConfiguration></TorchConfiguration> ID
        /// </summary>
        public int GroupingId { get; set; }
        /// <summary>
        /// The name of the predefined section.
        /// </summary>
        public string SectionName { get; set; }
        /// <summary>
        /// The sequence of the sections.
        /// </summary>
        public int SequenceOrder { get; set; }

        /// <summary>
        /// Return a new predefined section.
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="mergeProjectId"></param>
        /// <param name="groupingId"></param>
        /// <param name="sectionName"></param>
        /// <param name="sequenceOrder"></param>
        public TorchPredefinedSection(int sectionId, int mergeProjectId, int groupingId, string sectionName, int sequenceOrder)
        {
            SectionId = sectionId;
            MergeProjectId = mergeProjectId;
            GroupingId = groupingId;
            SectionName = sectionName;
            SequenceOrder = sequenceOrder;
        }
    }
}
