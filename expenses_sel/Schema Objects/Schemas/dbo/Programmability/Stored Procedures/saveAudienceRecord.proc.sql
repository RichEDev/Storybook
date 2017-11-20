CREATE PROCEDURE [dbo].[saveAudienceRecord]
@audienceId int,
@recordId int,
@tablename nvarchar(250),
@canView bit,
@canEdit bit,
@canAdd bit,
@canDelete bit,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @sql nvarchar(500);
	declare @sqlParms nvarchar(150);
	declare @count int;
	declare @otherRecCount int = -1;
	declare @returnID int;

	if @canView = 0 or @canEdit = 0 or @canDelete = 0
	begin
		-- Check at least one audience has view and edit rights on the record
		set @sql = 'select @retCount = count([id]) from [' + @tablename + '] where [canedit] = 1 and [canview] = 1 and [canDelete] = 1 and [audienceid] <> @audId and [parentID] = @pId';
		set @sqlParms = '@audId int, @pId int, @retCount int OUTPUT';
		exec sp_executesql @sql, @sqlParms, @audId = @audienceId, @pId = @recordId, @retCount = @otherRecCount output;
		
		if @otherRecCount = 0
		begin
			return -1;
			--set @canView = 1;
			--set @canEdit = 1;
			--set @canDelete = 1;
		end
	end

	set @returnID = 0;
		
	set @sql = 'select @retCount = count([id]) from [' + @tablename + '] where audienceId = @audId and [parentID] = @pId';
	set @sqlParms = '@audId int, @pId int, @retCount int OUTPUT';
	exec sp_executesql @sql, @sqlParms, @audId = @audienceId, @pId = @recordId, @retCount = @count output;

	if @count = 0
		begin
			set @sql = 'insert into [' + @tablename + '] (parentID, audienceID, canview, canedit, canadd, candelete, cacheExpiry) values (@recId, @audienceID, @canview, @canedit, @canadd, @candelete, getdate()); set @newIdentity = scope_identity();';
			set @sqlParms = '@recId int, @audienceID int, @canview bit, @canedit bit, @canadd bit, @candelete bit, @newIdentity int OUTPUT';
			exec sp_executesql @sql, @sqlParms, @recId = @recordId, @audienceID = @audienceId, @canview = @canView, @canedit = @canEdit, @canadd = @canAdd, @candelete = @canDelete, @newIdentity = @returnID output;

			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @returnID, @tablename, null;
		end
	else
	begin
		set @sql = 'update [' + @tablename + '] set canview = @canview, canedit = @canedit, canadd = @canadd, candelete = @candelete, CacheExpiry = getdate() where parentID = @recID and audienceID = @audienceID';
		set @sqlParms = '@recID int, @audienceID int, @canview bit, @canedit bit, @canadd bit, @candelete bit';
		exec sp_executesql @sql, @sqlParms, @recID = @recordId, @audienceID = @audienceId, @canview = @canView, @canedit = @canEdit, @canadd = @canAdd, @candelete = @canDelete;
		
		set @sql = 'select @ID = id from [' + @tablename + '] where audienceID = @audID and parentID = @pId';
		set @sqlParms = '@audID int, @pId int, @ID int OUTPUT';
		exec sp_executesql @sql, @sqlParms, @audID = @audienceId, @pId = @recordId, @ID = @returnID OUTPUT;		
	end
	
	-- Inform caller that canView and CanEdit cannot be revoked
	if @otherRecCount = 0
	begin
		set @returnID = -1;
	end
	
	return @returnID;
end 
