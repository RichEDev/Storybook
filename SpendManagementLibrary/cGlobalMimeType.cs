using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Object to store the details of the global mime type
    /// </summary>
    public class cGlobalMimeType
    {
        private Guid nGlobalMimeID;
        private string sFileExtension;
        private string sMimeHeader;
        private string sDescription;

        public cGlobalMimeType()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="GlobalMimeID"></param>
        /// <param name="FileExtension"></param>
        /// <param name="MimeHeader"></param>
        /// <param name="Description"></param>
        public cGlobalMimeType(Guid GlobalMimeID, string FileExtension, string MimeHeader, string Description)
        {
            nGlobalMimeID = GlobalMimeID;
            sFileExtension = FileExtension;
            sMimeHeader = MimeHeader;
            sDescription = Description;
        }

        #region properties

        /// <summary>
        /// Global ID of the mime type
        /// </summary>
        public Guid GlobalMimeID
        {
            get { return nGlobalMimeID; }
            set { nGlobalMimeID = value; }
        }

        /// <summary>
        /// File extension of the mime type
        /// </summary>
        public string FileExtension
        {
            get { return sFileExtension; }
            set { sFileExtension = value; }
        }

        /// <summary>
        /// Header for the mime type
        /// </summary>
        public string MimeHeader
        {
            get { return sMimeHeader; }
            set { sMimeHeader = value; }
        }

        /// <summary>
        /// Description of the Mime Type
        /// </summary>
        public string Description
        {
            get { return sDescription; }
            set { sDescription = value; }
        }

        #endregion
    }
}
