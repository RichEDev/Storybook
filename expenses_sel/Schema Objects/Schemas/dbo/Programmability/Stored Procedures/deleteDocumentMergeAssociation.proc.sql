
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
               select @attrName = 'att' + cast(attributeid as nvarchar) from custom_entity_attributes where entityid = @entityId and is_audit_identity = 1
               select @idAttr = attribute_name from custom_entity_attributes where fieldid = (select primarykey from tables where tableid = (select tableid from custom_entities where entityid = @entityId))
               select @tableName = plural_name from custom_entities where entityid = @entityId;
               set @sql = 'select @recname = ' + @attrName + ' from custom_' + replace(@tableName, ' ','_') + ' where ' + @idAttr + ' = @recid';
               set @sqlParams = '@recid int, @recname nvarchar(200) output';
               exec sp_executesql @sql, @sqlParams, @recordId, @recname = @recordName output;

               
               delete from document_merge_association where docmergeassociationid = @docmergeassociationid

               update document_templates set modifiedby = @CUemployeeID, modifieddate = GETDATE() where documentid = @documentId
               
               exec addDeleteEntryWithValueToAuditLog  @CUemployeeID, @CUdelegateID, 103, @docmergeassociationid, @recordTitle, @recordName, null;
end
