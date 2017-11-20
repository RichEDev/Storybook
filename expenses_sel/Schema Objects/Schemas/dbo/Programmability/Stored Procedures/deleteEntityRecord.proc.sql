

CREATE PROCEDURE [dbo].[deleteEntityRecord]
@entityid int,
@recordMatchAttributeId int,
@recordid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
	declare @parentTable nvarchar(250)
	declare @parentTableId uniqueidentifier
	--declare @idattributeid int
	declare @hasAttachments bit

	set @hasAttachments = (select enableAttachments from custom_entities where entityid = @entityid)
	set @parentTableId = (select tableid from custom_entities where entityid = @entityid)
	set @parentTable = (select tablename from tables where tableid = @parentTableId)
	--set @idattributeid = (select attributeid from custom_entity_attributes where entityid = @entityid and is_key_field = 1)

	declare @sql nvarchar(3000)
	set @sql = 'delete from [' + @parentTable + '] where att' + cast(@recordMatchAttributeId as nvarchar) + ' = ' + cast(@recordid as nvarchar)

	exec sp_executesql @sql

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 102, @recordid, @parentTable, null;

	if @hasAttachments = 1
	begin
		-- delete any attachments for the record
		set @sql = 'delete from [' + @parentTable + '_attachmentData] where attachmentID in (select attachmentID from [' + @parentTable + '_attachments] where id = ' + cast(@recordid as nvarchar) + ')'
		exec sp_executesql @sql

		set @sql = 'delete from [' + @parentTable + '_attachments] where id = ' + cast(@recordid as nvarchar) + ')'
		exec sp_executesql @sql
	end
	return;

