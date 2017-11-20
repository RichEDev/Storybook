CREATE PROCEDURE [dbo].[deleteDocumentMergeAssociation]
@docmergeassociationid int,
@CUemployeeID int,
@CUdelegateID int
as
begin
   declare @documentId int;
   declare @recordTitle nvarchar(2000);
   declare @entityId int;
   declare @recordId int;
   declare @recordName nvarchar(200);
   declare @sql nvarchar(500);
   declare @sqlParams nvarchar(500);
   declare @attrName nvarchar(10);
   declare @idAttr nvarchar(10);
   declare @tableName nvarchar(200);
   
   select @entityId = entityid, @recordId = recordid from document_merge_association where docmergeassociationid = @docmergeassociationid
   select @documentid = documentid from document_merge_association where docmergeassociationid = @docmergeassociationid
   select @recordTitle = doc_name from document_templates where documentid = @documentId;
   select @attrName = 'att' + cast(attributeid as nvarchar) from [customEntityAttributes] where entityid = @entityId and is_audit_identity = 1
   select @idAttr = 'att' + cast(attributeid as nvarchar) from [customEntityAttributes] where fieldid = (select primarykey from tables where tableid = (select tableid from [customEntities] where entityid = @entityId))
   select @tableName = masterTableName from [customEntities] where entityid = @entityId;
   set @sql = 'select @recname = ' + @attrName + ' from [custom_' + @tableName + '] where ' + @idAttr + ' = @recid';
   set @sqlParams = '@recid int, @recname nvarchar(200) output';
   exec sp_executesql @sql, @sqlParams, @recordId, @recname = @recordName output;

   
   delete from document_merge_association where docmergeassociationid = @docmergeassociationid

   SET @sql = 'DELETE FROM [custom_' + CAST(@entityId AS NVARCHAR) + '_attachmentData] WHERE [attachmentID] IN (SELECT [attachmentId] FROM [custom_' + CAST(@entityId AS NVARCHAR) + '_attachments] WHERE [TorchGenerated] = 1 AND [id] = ' + CAST(@docmergeassociationid AS NVARCHAR) + ')';
   EXEC sp_executesql @sql;

   update document_templates set modifiedby = @CUemployeeID, modifieddate = GETDATE() where documentid = @documentId
   
   exec addDeleteEntryWithValueToAuditLog  @CUemployeeID, @CUdelegateID, 103, @docmergeassociationid, @recordTitle, @recordName, null;
end