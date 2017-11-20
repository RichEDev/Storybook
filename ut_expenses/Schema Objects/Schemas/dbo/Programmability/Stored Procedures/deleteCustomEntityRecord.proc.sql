CREATE PROCEDURE [dbo].[deleteCustomEntityRecord]
@entityid int,
@recordMatchAttributeId int,
@recordid int,
@CUemployeeID INT,
@CUdelegateID INT,
@recordTitle NVARCHAR(2000)
AS
	declare @parentTable nvarchar(250)
	declare @parentTableId uniqueidentifier
	--declare @idattributeid int
	declare @hasAttachments bit

	set @hasAttachments = (select enableAttachments from [customEntities] where entityid = @entityid)
	set @parentTableId = (select tableid from [customEntities] where entityid = @entityid)
	set @parentTable = (select tablename from tables where tableid = @parentTableId)
	--set @idattributeid = (select attributeid from custom_entity_attributes where entityid = @entityid and is_key_field = 1)

	declare @checkRefCode int = 0;
	exec @checkRefCode = dbo.checkReferencedBy @parentTableId, @recordid;

	if @checkRefCode = 0
	begin
		IF EXISTS(SELECT LockedBy, lockedDateTime FROM CustomEntityLocking WHERE customEntityId = @entityid AND entityId = @recordid AND lockeddatetime >  DATEADD(MINUTE,-1,GETUTCDATE()))
		BEGIN
			SET @checkRefCode = -20;
		END
		ELSE
		BEGIN
			declare @sql nvarchar(3000)
			set @sql = 'delete from [' + @parentTable + '] where att' + cast(@recordMatchAttributeId as nvarchar) + ' = ' + cast(@recordid as nvarchar)
			exec sp_executesql @sql

			exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 102, @recordid, @recordTitle, null;

			if @hasAttachments = 1
			begin
				-- delete any attachments for the record
				set @sql = 'delete from [' + @parentTable + '_attachmentData] where attachmentID in (select attachmentID from [' + @parentTable + '_attachments] where id = ' + cast(@recordid as nvarchar) + ')';
				print @sql;
				exec sp_executesql @sql

				set @sql = 'delete from [' + @parentTable + '_attachments] where id = ' + cast(@recordid as nvarchar);
				exec sp_executesql @sql
			end
		END
	end
	return @checkRefCode;