using SpendManagementLibrary.Interfaces;

namespace SpendManagementLibrary.GreenLight
{
    public class HTMLImageData
    {
        private byte[] _imgData;
        private IDBConnection _databaseConnection;
        public HTMLImageData(string fileID, string fileType, string fileName, IDBConnection databaseConnection)
        {
            this.fileID = fileID;
            this.fileType = fileType;
            this.fileName = fileName;
            this._databaseConnection = databaseConnection;
        }
        public string fileID { get; set; }

        /// <summary>
        /// A lazy loading way of accessing the data for this <see cref="HTMLImageData"></see>.
        /// </summary>
        public byte[] imageData
        {
            get
            {
                if (this._imgData != null) { return this._imgData; }

                const string Sql = "select imageBinary from CustomEntityImageData where fileID = @fileID";

                this._databaseConnection.sqlexecute.Parameters.Clear();
                this._databaseConnection.sqlexecute.Parameters.AddWithValue("@fileID", this.fileID);

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
        public string fileType { get; set; }
        public string fileName { get; set; }            
    }
}