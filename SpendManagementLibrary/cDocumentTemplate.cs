using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    public class cDocumentTemplate
    {
        private int nDocumentId;
        private string sDocName;
        private string sDocPath;
        private string sDocFilename;
        private string sDocDescription;
        private string sContentType;
        private DateTime? dtCreatedDate;
        private int? nCreatedBy;
        private DateTime? dtModifiedDate;
        private int? nModifiedBy;
        private int nMergeProjectId;
        private string sMergeProject;
        private List<cDocumentMergeAssociation> cDocumentMergeAssociations;

        #region properties
        /// <summary>
        /// Returns the document ID of the uploaded file
        /// </summary>
        public int DocumentId
        {
            get { return nDocumentId; }
            set { nDocumentId = value; }
        }
        /// <summary>
        /// Returns the name of the document uploaded
        /// </summary>
        public string DocumentName
        {
            get { return sDocName; }
        }
        /// <summary>
        /// Returns the path of the merge document
        /// </summary>
        public string DocumentPath
        {
            get { return sDocPath; }
        }
        /// <summary>
        /// Returns the filename of the uploaded file
        /// </summary>
        public string DocumentFilename
        {
            get { return sDocFilename; }
        }
        /// <summary>
        /// Returns the description supplied for the uploaded file
        /// </summary>
        public string DocumentDescription
        {
            get { return sDocDescription; }
        }
        /// <summary>
        /// Returns the Content Type (MIME) of the uploaded file
        /// </summary>
        public string DocumentContentType
        {
            get { return sContentType; }
        }
        /// <summary>
        /// Returns the creation date of the file upload
        /// </summary>
        public DateTime? CreatedDate
        {
            get { return dtCreatedDate; }
        }
        /// <summary>
        /// Returns the user ID of who originally uploaded the file
        /// </summary>
        public int? CreatedBy
        {
            get { return nCreatedBy; }
        }
        /// <summary>
        /// Returns the date the upload file record was last modified
        /// </summary>
        public DateTime? ModifiedDate
        {
            get { return dtModifiedDate; }
        }
        /// <summary>
        /// Returns the user ID of who last modified the upload record
        /// </summary>
        public int? ModifiedBy
        {
            get { return nModifiedBy; }
        }
        /// <summary>
        /// Returns the merge configuration id for the template
        /// </summary>
        public int MergeProjectId
        {
            get { return nMergeProjectId; }
        }

        public List<cDocumentMergeAssociation> DocumentMergeAssociations
        {
            get
            {
                return cDocumentMergeAssociations;
            }
            set
            {
                cDocumentMergeAssociations = value;
            }
        }
        #endregion

        /// <summary>
        /// cDocumentTemplate, instatiates an instance of a document template record
        /// </summary>
        /// <param name="docid">Unique document ID</param>
        /// <param name="documentName">Name or title assigned to the uploaded document</param>
        /// <param name="path">Original path of document that was uploaded</param>
        /// <param name="filename">Original filename of the document that was uploaded</param>
        /// <param name="description">Description assigned to the uploaded document</param>
        /// <param name="contenttype">Content Type (MIME) of the document uploaded</param>
        /// <param name="createddate">Date document originally uploaded</param>
        /// <param name="createdby">User ID who originally uploaded document</param>
        /// <param name="modifieddate">Date template record was last modified</param>
        /// <param name="modifiedby">User ID of who last modified template record</param>
        public cDocumentTemplate(int docid, string documentName, string path, string filename, string description, string contenttype, DateTime? createddate, int? createdby, DateTime? modifieddate, int? modifiedby, int mergeprojectid)
        {
            nDocumentId = docid;
            sDocName = documentName;
            sDocPath = path;
            sDocFilename = filename;
            sDocDescription = description;
            sContentType = contenttype;
            dtCreatedDate = createddate;
            nCreatedBy = createdby;
            dtModifiedDate = modifieddate;
            nModifiedBy = modifiedby;
            nMergeProjectId = mergeprojectid;
            
            cDocumentMergeAssociations = new List<cDocumentMergeAssociation>();
        }
    }
}
