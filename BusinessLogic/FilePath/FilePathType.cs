namespace BusinessLogic.FilePath
{
    /// <summary>
    /// Represents the columns that contain paths in the metabase table dbo.databases.
    /// </summary>
    public enum FilePathType
    {
        /// <summary>
        /// The receipts path column.
        /// </summary>
        Receipt = 0,

        /// <summary>
        /// The card template path column.
        /// </summary>
        CardTemplate = 1,

        /// <summary>
        /// The offline update path column.
        /// </summary>
        OfflineUpdate = 2,

        /// <summary>
        /// The policy file path column.
        /// </summary>
        PolicyFile = 3,

        /// <summary>
        /// The car document path column.
        /// </summary>
        CarDocument = 4,

        /// <summary>
        /// The logo path column.
        /// </summary>
        Logo = 5,

        /// <summary>
        /// The attachments path column.
        /// </summary>
        Attachments = 6
    }
}
