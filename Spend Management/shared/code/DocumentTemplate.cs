using System.Text;

namespace Spend_Management.shared.code
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    using Syncfusion.DocIO;
    using Syncfusion.DocIO.DLS;

    /// <summary>
    /// Export document types
    /// </summary>
    public enum ExportDocumentType
    {
        MS_Word_DOC = 1,
        MS_Word_DOCX
    }

    public class DocumentTemplate
    {
        System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
        Dictionary<int, cDocumentTemplate> templates;
        private int nAccountId;
        private int? nSubAccountId;
        private int nEmployeeId;

        /// <summary>
        /// cDocumentTemplates, loads and caches merge templates for the specified customer database
        /// </summary>
        /// <param name="accountid">Database accountid from the metabase</param>
        public DocumentTemplate(int accountid, int? subAccountId, int employeeID)
        {
            this.nAccountId = accountid;
            this.nEmployeeId = employeeID;
            this.nSubAccountId = subAccountId;

            this.InitialiseData();
        }

        private void InitialiseData()
        {
            this.templates = (Dictionary<int, cDocumentTemplate>)this.Cache["doc_templates_" + this.accountid];
            if (this.templates == null)
            {
                this.templates = this.CacheItems();
            }
        }

        #region properties
        /// <summary>
        /// Returns the current metabase customer ID
        /// </summary>
        public int accountid
        {
            get { return this.nAccountId; }
        }
        public int? subAccountId
        {
            get { return this.nSubAccountId; }
        }
        public int employeeId
        {
            get { return this.nEmployeeId; }
        }
        #endregion

        private Dictionary<int, cDocumentTemplate> CacheItems()
        {
            Dictionary<int, cDocumentTemplate> tmpTemplates = new Dictionary<int, cDocumentTemplate>();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(this.accountid));

            string sql = "select documentid, doc_name, doc_path, doc_filename, doc_description, doc_contenttype, createddate, createdby, modifieddate, modifiedby, mergeprojectid from dbo.document_templates";
            db.sqlexecute.CommandText = sql;
            System.Web.Caching.SqlCacheDependency dep = null;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                dep = new System.Web.Caching.SqlCacheDependency(db.sqlexecute);
            }

            using (SqlDataReader reader = db.GetReader(sql))
            {
                while (reader.Read())
                {
                    int documentid = reader.GetInt32(reader.GetOrdinal("documentid"));
                    string doc_name = reader.GetString(reader.GetOrdinal("doc_name"));
                    string doc_path = reader.GetString(reader.GetOrdinal("doc_path"));
                    string doc_filename = reader.GetString(reader.GetOrdinal("doc_filename"));
                    string doc_description = "";
                    if (!reader.IsDBNull(reader.GetOrdinal("doc_description")))
                    {
                        doc_description = reader.GetString(reader.GetOrdinal("doc_description"));
                    }
                    string doc_contenttype = reader.GetString(reader.GetOrdinal("doc_contenttype"));
                    DateTime? createddate = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("createddate")))
                    {
                        createddate = reader.GetDateTime(reader.GetOrdinal("createddate"));
                    }
                    int? createdby = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("createdby")))
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    DateTime? modifieddate = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("modifieddate")))
                    {
                        modifieddate = reader.GetDateTime(reader.GetOrdinal("modifieddate"));
                    }
                    int? modifiedby = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }
                    int mergeprojectid = reader.GetInt32(10);
                    var doc = new cDocumentTemplate(documentid, doc_name, doc_path, doc_filename, doc_description, doc_contenttype, createddate, createdby, modifieddate, modifiedby, mergeprojectid);

                    var db1 = new DBConnection(cAccounts.getConnectionString(this.accountid));
                    //                               0                      1           2           3       4               5       6               7       
                    string sqlAssociations = "select docmergeassociationid, documentid, entityid, recordid, createddate, createdby, modifieddate, modifiedby from dbo.document_merge_association where documentid = " + documentid;
                    db1.sqlexecute.CommandText = sqlAssociations;
                    using (SqlDataReader readerassocs = db1.GetReader(sqlAssociations))
                    {
                        while (readerassocs.Read())
                        {
                            int? modby = null;
                            if (!readerassocs.IsDBNull(7))
                                modby = readerassocs.GetInt32(7);
                            DateTime? moddate = null;
                            if (!readerassocs.IsDBNull(6))
                                moddate = readerassocs.GetDateTime(6);
                            doc.DocumentMergeAssociations.Add(new cDocumentMergeAssociation
                            {
                                CreatedBy = readerassocs.GetInt32(5),
                                CreatedDate = readerassocs.GetDateTime(4),
                                DocMergeAssociationId = readerassocs.GetInt32(0),
                                DocumentId = readerassocs.GetInt32(1),
                                EntityId = readerassocs.GetInt32(2),
                                ModifiedBy = modby,
                                ModifiedDate = moddate,
                                RecordId = readerassocs.GetInt32(3)
                            });
                        }
                        readerassocs.Close();
                    }

                    tmpTemplates.Add(documentid, doc);
                }
                reader.Close();
            }

            if (tmpTemplates.Count > 0 && GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                this.Cache.Insert("doc_templates_" + this.accountid, tmpTemplates, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short), System.Web.Caching.CacheItemPriority.Default, null);
            }

            return tmpTemplates;
        }

        /// <summary>
        /// getDocument: Retrieve the actual document file from the database
        /// </summary>
        /// <param name="documentId">Database ID of document you wish to retrieve</param>
        /// <returns>A byte array document image from the dabase</returns>
        public byte[] getDocument(int documentId)
        {
            cDocumentTemplate docTemplate = this.getTemplateById(documentId);
            var db = new DBConnection(cAccounts.getConnectionString(this.nAccountId));

            const string sql = "select document_data from dbo.document_template_data where documentid = @docid";
            db.sqlexecute.Parameters.AddWithValue("@docid", documentId);
            byte[] docStream = db.getImageData(sql);

            db.sqlexecute.Parameters.Clear();

            return docStream;
        }

        /// <summary>
        /// getTemplateById: Retrieve a document template from the database
        /// </summary>
        /// <param name="documentId">Database ID of document you wish to retrieve</param>
        /// <returns>Document template in the cDocumentTemplate class structure. Returns NULL if not found.</returns>
        public cDocumentTemplate getTemplateById(int documentId)
        {
            if (this.templates.ContainsKey(documentId))
            {
                return this.templates[documentId];
            }
                return null;
        }

        /// <summary>
        /// Gets an array of cDocumentMergeAssociation identifiers for a single record of a custom entity
        /// </summary>
        /// <param name="entityId">The entity identifier</param>
        /// <param name="recordId">The record identifier</param>
        /// <returns>An array of <see cref="cDocumentMergeAssociation" />identifiers</returns>
        public int[] GetDocumentTemplateAssociationIdsByEntityRecord(int entityId, int recordId)
        {
            var list = new List<int>();
            foreach (cDocumentTemplate template in this.templates.Values)
            {
                foreach (cDocumentMergeAssociation association in template.DocumentMergeAssociations)
                {
                    if (association.EntityId == entityId && association.RecordId == recordId)
                    {
                        list.Add(association.DocMergeAssociationId);
                    }
                }
            }

            return list.ToArray();
        } 

        /// <summary>
        /// documentExists: Check whether a document of the specified name already exists
        /// </summary>
        /// <param name="documentName">Name or title of document you wish to check</param>
        /// <returns>TRUE if document exists, FALSE otherwise</returns>
        public bool documentExists(string documentName, int docID)
        {
            bool found = false;

            foreach (KeyValuePair<int, cDocumentTemplate> i in this.templates)
            {
                cDocumentTemplate doc = i.Value;

                if (doc.DocumentName == documentName && doc.DocumentId != docID)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Chaeck if a document merge association already exists in the merge associations collections of the templates.
        /// </summary>
        /// <param name="documentId">Document Id</param>
        /// <param name="entityId">Entity Id</param>
        /// <param name="RecordId">Record Id within the Entity</param>
        /// <returns>Whether or not the record is found in the merge associations collections</returns>
        private bool documentAssociationExists(int documentId, int entityId, int RecordId)
        {
            bool bResult = false;
            var x = (from y in this.templates[documentId].DocumentMergeAssociations
                     where y.EntityId == entityId && y.RecordId == RecordId
                     select y).FirstOrDefault();
            if (x != null)
                bResult = true;

            return bResult;
        }

        /// <summary>
        /// StoreDocument: Save a document to the database
        /// </summary>
        /// <param name="docTemplate">Document template definition in the form of a cDocumentTemplate class structure</param>
        /// <param name="document">Document to store in the form of a byte array</param>
        /// <returns>Database ID of the document stored</returns>
        public int StoreDocument(cDocumentTemplate docTemplate, byte[] document)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            int nDocId;
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
            if (!this.documentExists(docTemplate.DocumentName, docTemplate.DocumentId))
            {
                    connection.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
                    connection.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;
                    connection.sqlexecute.Parameters.AddWithValue("@documentid", docTemplate.DocumentId);
                    connection.sqlexecute.Parameters.AddWithValue("@documentName", docTemplate.DocumentName);
                    connection.sqlexecute.Parameters.AddWithValue("@documentPath", docTemplate.DocumentPath);
                    connection.sqlexecute.Parameters.AddWithValue("@documentFilename", docTemplate.DocumentFilename);
                    connection.sqlexecute.Parameters.AddWithValue("@documentDescription", docTemplate.DocumentDescription);
                    connection.sqlexecute.Parameters.AddWithValue("@documentContentType", docTemplate.DocumentContentType);
                    connection.sqlexecute.Parameters.AddWithValue("@mergeProjectId", docTemplate.MergeProjectId);
                    connection.sqlexecute.Parameters.Add("@documentImage", SqlDbType.Binary, -1);
                if (document != null && document.Length > 0)
                {
                        connection.sqlexecute.Parameters["@documentImage"].Value = document;
                }
                else
                {
                        connection.sqlexecute.Parameters["@documentImage"].Value = DBNull.Value;
                }

                    connection.sqlexecute.Parameters.AddWithValue("@userid", curUser.EmployeeID);
                    connection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", curUser.EmployeeID);

                    if (curUser.isDelegate)
                {
                        connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", curUser.Delegate.EmployeeID);
                }
                else
                {
                        connection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                    connection.ExecuteProc("saveDocumentTemplate");

                    if (connection.sqlexecute.Parameters["@ReturnId"].Value == null)
                {
                    nDocId = 0;
                }
                else
                {
                        docTemplate.DocumentId = (int)connection.sqlexecute.Parameters["@ReturnId"].Value;
                    nDocId = docTemplate.DocumentId;

                    if (docTemplate.DocumentId > 0)
                    {
                        if (this.templates.ContainsKey(docTemplate.DocumentId))
                        {
                            this.templates[docTemplate.DocumentId] = docTemplate;
                        }
                        else
                        {
                            this.templates.Add(docTemplate.DocumentId, docTemplate);
                        }
                    }

                        connection.sqlexecute.Parameters.Clear();
                }
            }
            else
            {
                nDocId = -1;
            }
            }

            return nDocId;
        }

        private static byte[] ConvertStreamToByteArray(Stream input)
        {
            var buffer = new byte[16 * 1024];

            using (var stream = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, read);
                }

                return stream.ToArray();
            }
        }

        private int SaveDocumentSection(
            cDocumentTemplate template,
            string sectionName,
            byte[] documentData,
            bool firstPass = false)
        {
            int newDocumentId;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                connection.sqlexecute.Parameters.Add("@ReturnId", SqlDbType.Int);
                connection.sqlexecute.Parameters["@ReturnId"].Direction = ParameterDirection.ReturnValue;
                connection.sqlexecute.Parameters.AddWithValue("@mergeProjectId", template.MergeProjectId);
                connection.sqlexecute.Parameters.AddWithValue("@documentPartName", sectionName);

                if (documentData != null && documentData.Length > 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@documentData", documentData);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@documentData", DBNull.Value);
                }

                connection.sqlexecute.Parameters.AddWithValue("@firstPass", firstPass);
                connection.ExecuteProc("saveDocumentSection");

                if (connection.sqlexecute.Parameters["@ReturnId"].Value == null)
                {
                    newDocumentId = 0;
                }
                else
                {
                    newDocumentId = (int)connection.sqlexecute.Parameters["@ReturnId"].Value;
                    connection.sqlexecute.Parameters.Clear();
                }
            }

            return newDocumentId;
        }

        private void SplitTemplateIntoSeperateFiles(cDocumentTemplate template, byte[] documentBytes)
        {
            WordDocument document;
            using (Stream stream = new MemoryStream(documentBytes))
            {
                document = new WordDocument(stream);
            }

            var newDocument = document.Clone();
            newDocument.ChildEntities.Clear();
            BookmarkCollection bookmarks = document.Bookmarks;
            int currentBookmarkPosition = 0;
            string currentBookmarkName = bookmarks[currentBookmarkPosition].Name;
            bool firstPass = true;
            bool firstDatabasePass = true;

            foreach (WSection section in document.Sections)
            {
                string nextBookmarkName;
                if (!SectionContainsABookmark(section, firstPass, out nextBookmarkName) || firstPass)
                {
                    newDocument.Sections.Add(section.Clone());
                }
                else
                {
                    using (var newStream = new MemoryStream())
                    {
                        newDocument.Save(newStream, FormatType.Docx);
                        byte[] newDocumentByteData = newStream.ToArray();
                        SaveDocumentSection(template, currentBookmarkName, newDocumentByteData, firstDatabasePass);
                        firstDatabasePass = false;
                    }

                    currentBookmarkPosition++;
                    currentBookmarkName = nextBookmarkName;
                    newDocument.Close();
                    newDocument = document.Clone();
                    newDocument.ChildEntities.Clear();
                    newDocument.Sections.Add(section.Clone());
                }

                firstPass = false;
            }

            newDocument = null;
            document.Close();
            document = null;
        }
        private static bool SectionContainsABookmark(Entity entity, bool firstPass, out string nextBookmarkName)
        {
            switch (entity.EntityType)
            {
                case EntityType.Section:
                    var section = (WSection)entity;

                    foreach (Entity entity1 in section.ChildEntities)
                    {
                        if (SectionContainsABookmark(entity1, firstPass, out nextBookmarkName))
                        {
                            return true;
                        }
                    }

                    break;
                case EntityType.TextBody:
                    var textBody = (WTextBody)entity;
                    foreach (Entity entity1 in textBody.ChildEntities)
                    {
                        if (SectionContainsABookmark(entity1, firstPass, out nextBookmarkName))
                        {
                            return true;
                        }
                    }

                    break;
                case EntityType.Paragraph:
                    var paragraph = (WParagraph)entity;
                    foreach (Entity entity1 in paragraph.ChildEntities)
                    {
                        if (SectionContainsABookmark(entity1, firstPass, out nextBookmarkName))
                        {
                            return true;
                        }
                    }
                    break;
                case EntityType.BookmarkStart:
                    if (!firstPass)
                    {
                        var bookmarkStart = (BookmarkStart)entity;
                        nextBookmarkName = bookmarkStart.Name;
                        return true;
                    }

                    break;
            }

            nextBookmarkName = string.Empty;

            return false;
        }

        /// <summary>
        /// Save a document merge association record
        /// </summary>
        /// <param name="documentId">Template Document Id</param>
        /// <param name="entityId">The Custom Entity Id</param>
        /// <param name="RecordId">The Record Id under the Entity</param>
        /// <returns></returns>
        public int SaveDocumentMergeAssociation(int documentId, int entityId, int RecordId)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            var db = new DBConnection(cAccounts.getConnectionString(this.accountid));
            int nDocAssociationId = 0;
            DateTime dCreateDate = DateTime.Now;

            if (!this.documentAssociationExists(documentId, entityId, RecordId))
            {
                db.sqlexecute.Parameters.Add("@ReturnId", System.Data.SqlDbType.Int);
                db.sqlexecute.Parameters["@ReturnId"].Direction = System.Data.ParameterDirection.ReturnValue;
                db.sqlexecute.Parameters.AddWithValue("@documentid", documentId);
                db.sqlexecute.Parameters.AddWithValue("@entityid", entityId);
                db.sqlexecute.Parameters.AddWithValue("@recordid", RecordId);
                db.sqlexecute.Parameters.AddWithValue("@createddate", dCreateDate);
                db.sqlexecute.Parameters.AddWithValue("@userid", curUser.EmployeeID);
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", curUser.EmployeeID);
                if (curUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", curUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                db.ExecuteProc("saveDocumentTemplateAssociation ");
                nDocAssociationId = (int)db.sqlexecute.Parameters["@ReturnId"].Value;
                db.sqlexecute.Parameters.Clear();

                var mergeAssoc = new cDocumentMergeAssociation
                {
                    DocMergeAssociationId = nDocAssociationId,
                    DocumentId = documentId,
                    EntityId = entityId,
                    RecordId = RecordId,
                    CreatedBy = curUser.EmployeeID,
                    CreatedDate = dCreateDate,
                    ModifiedBy = null,
                    ModifiedDate = null
                }; 
                
                this.CacheItems();
            }
            else
            {
                nDocAssociationId = -1;
            }
            return nDocAssociationId;
        }

        /// <summary>
        /// DeleteDocument: Delete a document and associated template record from the database permanently
        /// </summary>
        /// <param name="documentId">Database ID of document you wish to delete</param>
        public void DeleteDocument(int documentId)
        {
            var db = new DBConnection(cAccounts.getConnectionString(this.accountid));

            db.sqlexecute.Parameters.AddWithValue("@documentid", documentId);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);

            if (currentUser.isDelegate)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            db.ExecuteProc("deleteDocumentTemplate");
            db.sqlexecute.Parameters.Clear();
            return;
        }

        /// <summary>
        /// Delete a document merge association record
        /// </summary>
        /// <param name="docMergeAssociationId">Id for the document merge association record</param>
        public void DeleteDocumentTemplateAssociation(int docMergeAssociationId)
        {
            var db = new DBConnection(cAccounts.getConnectionString(this.accountid));

            db.sqlexecute.Parameters.AddWithValue("@docmergeassociationid", docMergeAssociationId);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);

            if (currentUser.isDelegate)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            db.ExecuteProc("deleteDocumentMergeAssociation");
            db.sqlexecute.Parameters.Clear();

            this.CacheItems();
        }

        /// <summary>
        /// getListItems: Returns list of templates for populating a drop down list
        /// </summary>
        /// <param name="includeNoneEntry">True will include a [None] option with a value of zero</param>
        /// <returns>Array of ListItems</returns>
        public ListItem[] getListItems(bool includeNoneEntry)
        {
            int count = this.templates.Count;
            if (includeNoneEntry)
            {
                count++;
            }

            ListItem[] items = new ListItem[count];
            int idx = 0;

            if (includeNoneEntry)
            {
                items[idx++] = new ListItem("[None]", "0");
            }

            foreach (KeyValuePair<int, cDocumentTemplate> i in this.templates)
            {
                cDocumentTemplate curTemplate = (cDocumentTemplate)i.Value;
                ListItem newItem = new ListItem(curTemplate.DocumentName, curTemplate.DocumentId.ToString());
                items[idx++] = newItem;
            }

            return items;
        }

        public Stream getDocumentStream(int documentId)
        {
            Stream stream = null;

            if (this.templates.ContainsKey(documentId))
            {
                byte[] doc = this.getDocument(documentId);
                stream = new MemoryStream(doc.Length);
                stream.Write(doc, 0, Convert.ToInt32(doc.Length));
                stream.Seek(0, SeekOrigin.Begin);
            }

            return stream;
        }

        /// <summary>
        /// Returns a word document for the given document id and merge project id. 
        /// </summary>
        /// <param name="documentId">the document template id</param>
        /// <param name="mergeProjectId">the merge project id</param>
        /// <param name="formatType">The format type for the document</param>
        /// <returns>Syncfusion Word Document</returns>
        public WordDocument GetDocumentForMergeProject(int documentId, int mergeProjectId, FormatType formatType)
        {
            using (Stream templateStream = this.getDocumentStream(documentId))
            {
                var templateDocument = new WordDocument(templateStream, formatType);
                templateStream.Close();
                return templateDocument;
            }
        }


        /// <summary>
        /// Return the grid for current document merge associations
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="docid"> Document ID to match</param>
        /// <param name="allowDelete">True if user is allowed to delete</param>
        /// <returns></returns>
        public string[] GetMergeAssociationGrid(CurrentUser user, int docid, bool allowDelete)
        {
            var entitiyIDs = this.GetAllDocMergeAccessEntitiyIDs();

            var sql = new StringBuilder();

            foreach (int entitiyId in entitiyIDs)
            {
                if (sql.Length != 0)
                {
                    sql.Append(" union ");
                }

                sql.Append(this.getMergeAssociationSql(entitiyId));
                if (docid == 0)
                {
                    sql.Append(string.Format(" WHERE  document_merge_association.entityid = {0}",  entitiyId));
                }
                else
                {
                    sql.Append(string.Format(" WHERE document_merge_association.documentid = {0} AND document_merge_association.entityid = {1}", docid, entitiyId));    
                }
                
            }

            var dataSet = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)).GetDataSet(sql.ToString());

            if (dataSet.Tables.Count == 0)
            {
                var table = new DataTable();
                table.Columns.Add("docmergeassociationid");
                table.Columns.Add("Document Name");
                table.Columns.Add("GreenLight");
                table.Columns.Add("Project / Target Name");
                dataSet.Tables.Add(table);
            }

            var grid = new cGridNew(user, dataSet, "gMergeAssociationChoices");

            var fields = new cFields(user.AccountID);
            grid.KeyField = "docmergeassociationid";
            grid.getColumnByName("docmergeassociationid").hidden = true;
            grid.enabledeleting = allowDelete;
            grid.deletelink = "javascript:deleteDocMergeAssociation('{docmergeassociationid}');";
            grid.CssClass = "datatbl";
            grid.EmptyText = "There are no associations with this template.";
            grid.addFilter(fields.GetFieldByID(new Guid("61745E03-1B80-44CD-9CE9-E8A761B5D40D")), ConditionType.Equals, new object[] { docid }, null, ConditionJoiner.None);

            var retVals = new List<string> {grid.GridID};
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        /// Save a selection of record ids against an entity. Any errors are returned in a list.
        /// </summary>
        /// <param name="docid">Document Template ID</param>
        /// <param name="recordIds">Entity Record ID</param>
        /// <returns>List of errors</returns>
        public List<string> SaveNewAssociations(int docid, string[] recordIds)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var templates = new DocumentTemplate(user.AccountID, user.CurrentSubAccountId, user.EmployeeID);
            List<cDocumentMergeAssociation> currentDocAssocs = templates.getTemplateById(docid).DocumentMergeAssociations;
            var messageList = new List<string>();

            foreach (string sId in recordIds)
            {
                var splitEntityRecordId = sId.Split('#');
                if (splitEntityRecordId.Length == 2)
                {
                    int selectedRecordId = Convert.ToInt32(splitEntityRecordId[1]);
                    int selectedEntityId = -1;
                    if (int.TryParse(splitEntityRecordId[0], out selectedEntityId))
                    {

                        cDocumentMergeAssociation dma =
                            currentDocAssocs.FirstOrDefault(
                                x => x.RecordId == selectedRecordId && x.EntityId == selectedEntityId);
                        if (dma == null)
                        {
                            var nNewId = templates.SaveDocumentMergeAssociation(docid, selectedEntityId,
                                selectedRecordId);
                            messageList.Add(nNewId == -1 ? "error" : "success");
                    }
                    else
                {
                            messageList.Add("duplicate");
                }
            }
        }
            }
            return messageList;
        }

        /// <summary>
        /// Creates the cGridNew for available associations against a document template. This does not return already existing associations
        /// </summary>
        /// <param name="docid">Template Document ID</param>
        /// <param name="user">User account</param>
        /// <param name="entityid">The Entity ID</param>
        /// <returns>HTML string for the available associations grid</returns>
        public string[] GetAvailableAssociationsGrid(int docid, CurrentUser user)
        {
            var entitiyIDs = this.GetAllDocMergeAccessEntitiyIDs();

            var sSql = new StringBuilder();
            foreach (int entitiyId in entitiyIDs)
            {
                if (sSql.Length != 0)
                {
                    sSql.Append(" union ");
                }

                sSql.Append(this.getAvailableAssociationInfo(user, docid, entitiyId));
            }

            var dataSet = new DatabaseConnection(cAccounts.getConnectionString(accountid)).GetDataSet(sSql.ToString());
            if (dataSet.Tables.Count == 0)
            {
                var table = new DataTable();
                table.Columns.Add("RecordId");
                table.Columns.Add("GreenLight");
                table.Columns.Add("Project / Target Name");
                dataSet.Tables.Add(table);
            }

            var grid = new cGridNew(user, dataSet, "gMergeAssociations")
            {
                KeyField = "RecordId",
                enablepaging = true,
                pagesize = GlobalVariables.DefaultModalGridPageSize,
                GridSelectType = GridSelectType.CheckBox,
                EnableSelect = true,
                enabledeleting = false,
                CssClass = "datatbl",
                EmptyText = "There are no associations available for this template."
            };

            grid.getColumnByName("RecordId").hidden = true;

            var retVals = new List<string> {grid.GridID};
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        private string getAvailableAssociationInfo(ICurrentUser user,int docid, int entityid)
        {
            var customEntities = new cCustomEntities(user).getEntityById(entityid);
            cAttribute attrIdentifier = customEntities.getAuditIdentifier();
            string sTableName = customEntities.table.TableName;
            string sPrimaryKey = customEntities.table.GetPrimaryKey().FieldName;
            string sDescriptorField = "att" + attrIdentifier.attributeid.ToString();

            return
                string.Format(
                    "select '{0}#' + CAST( {1} as NVarchar(10)) AS RecordId, customentities.entity_name as GreenLight, CAST({2} as Nvarchar(100)) COLLATE DATABASE_DEFAULT AS [Project / Target Name] from {3} inner join customentities on customentities.entityid = {0}  where {1} not in (select recordid from document_merge_association where entityid={0} and documentid={4}) ",
                    entityid, sPrimaryKey, sDescriptorField, sTableName, docid);
        }

        /// <summary>
        /// Return a list of all custom entity IDs that have "allow torch" set.
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllDocMergeAccessEntitiyIDs()
                               {
            var currentUser = cMisc.GetCurrentUser();
            var customEntities = new cCustomEntities(currentUser);
            var entityValues = customEntities.CustomEntities.Values;
            var result = (from entity in entityValues where entity.AllowMergeConfigAccess && !entity.IsSystemView select entity.entityid).ToList();
            return result;
        }

        private StringBuilder getMergeAssociationSql(int entityid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntity entity = new cCustomEntities(currentUser).getEntityById(entityid);
            cAttribute attrIdentifier = entity.getAuditIdentifier();
            string sTableName = entity.table.TableName;
            string sPrimaryKey = entity.table.GetPrimaryKey().FieldName;
            string sDescriptorField = "att" + attrIdentifier.attributeid.ToString();

            StringBuilder sSql = new StringBuilder(string.Format("select document_merge_association.docmergeassociationid, doc_name as [Document Name], customentities.entity_name AS GreenLight, CAST({0}.{1} AS Nvarchar(100)) COLLATE DATABASE_DEFAULT AS [Project / Target Name] from document_merge_association ", sTableName, sDescriptorField));
            sSql.Append("inner join document_templates on document_templates.documentid = document_merge_association.documentid ");
            sSql.Append(string.Format("inner join {0} on {0}.{1} = document_merge_association.recordid ", sTableName, sPrimaryKey));
            sSql.Append(string.Format("inner join customEntities on customEntities.entityid = {0} ", entityid));
            return sSql;

        }
    }
}
