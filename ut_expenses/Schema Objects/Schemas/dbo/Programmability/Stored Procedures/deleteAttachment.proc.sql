

CREATE procedure [dbo].[deleteAttachment]

@tablename nvarchar(300),

@attachmentid int,
@CUemployeeID INT,
@CUdelegateID INT

as

begin

      DECLARE @sql nvarchar(2500)

 

      SET @sql = 'DELETE FROM [' + @tablename + '] WHERE attachmentID = ' + cast(@attachmentid as nvarchar)

      EXECUTE sp_executesql @sql

 

      SET @sql = 'DELETE FROM [' + replace(@tablename,'_attachments','_attachmentData') + '] WHERE attachmentID = ' + cast(@attachmentid as nvarchar)

      EXECUTE sp_executesql @sql

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @attachmentid, @tablename, null;
end
