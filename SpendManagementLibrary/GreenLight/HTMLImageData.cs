using SpendManagementLibrary.Interfaces;

namespace SpendManagementLibrary.GreenLight
{
    /// <summary>
    /// A class to manage Html Image data for greenlight entities.
    /// </summary>
    public class HtmlImageData
    {
        private byte[] _imgData;

        private readonly IDBConnection _databaseConnection;

        /// <summary>
        /// Create a new instance of <see cref="HtmlImageData"/>
        /// </summary>
        /// <param name="fileId">The Guid ID as a string</param>
        /// <param name="fileType">The type of the file (file extention)</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="databaseConnection">An instance of <see cref="IDBConnection"/></param>
        /// <param name="entityName">The name of the entity this file is related to</param>
        /// <param name="displayName">The display name of the attribute the file is related to.</param>
        public HtmlImageData(string fileId, string fileType, string fileName, IDBConnection databaseConnection, string entityName, string displayName)
        {
            this.FileId = fileId;
            this.FileType = fileType;
            this.FileName = fileName;
            this.EntityName = entityName;
            this.DisplayName = displayName;
            this._databaseConnection = databaseConnection;
        }

        /// <summary>
        /// The Guid ID of the file as a string
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// The type (file extention) of the file
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// The filename
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The name of the entity that this file is related to.
        /// </summary>
        public string EntityName { get; }

        /// <summary>
        /// The name of the attribute that this file is related to.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// A lazy loading way of accessing the data for this <see cref="HtmlImageData"></see>.
        /// </summary>
        public byte[] ImageData
        {
            get
            {
                if (this._imgData != null) { return this._imgData; }

                const string Sql = "select imageBinary from CustomEntityImageData where fileID = @fileID";

                this._databaseConnection.sqlexecute.Parameters.Clear();
                this._databaseConnection.sqlexecute.Parameters.AddWithValue("@fileID", this.FileId);

                using (var reader = this._databaseConnection.GetReader(Sql))
                {
                    while (reader.Read())
                    {
                        this._imgData = (byte[]) reader["imageBinary"];
                    }
                }

                return this._imgData;
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object Filename (Entityname, attributename).</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"{this.FileName} / {this.EntityName} ({this.DisplayName})";
        }
    }
}