CREATE PROCEDURE [dbo].[saveDocumentTemplateAssociation]
@documentid int,
@entityid int,
@recordid int,
@createddate smalldatetime,
@userid int,
@CUemployeeID int,
@CUdelegateID int
AS
BEGIN
               declare @returnDocId int
               declare @count int
               declare @docName nvarchar(150)

               SET @count = (SELECT COUNT(docmergeassociationid) FROM document_merge_association WHERE documentid = @documentid AND entityid = @entityid AND recordid = @recordid);
               IF @count > 0
                              RETURN -1;

               SET @docName = (select doc_name from document_templates where documentid = @documentid)
               insert into document_merge_association (documentid, entityid, recordid, createddate, createdby, modifieddate, modifiedby)
               values (@documentid, @entityid, @recordid, @createddate, @userid, @createddate, @userid)

               set @returnDocId = scope_identity()
               
               update document_templates set modifiedby = @userid, modifieddate = @createddate where documentid = @documentid

               exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 103, @returnDocId, @docName, null;

               return @returnDocId

END
