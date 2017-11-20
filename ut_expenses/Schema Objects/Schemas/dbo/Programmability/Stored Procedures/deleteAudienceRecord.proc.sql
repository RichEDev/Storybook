CREATE procedure [dbo].[deleteAudienceRecord]
@audienceRecordId int,
@tablename nvarchar(500),
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @sql nvarchar(500);
	declare @sqlParms nvarchar(150);
	declare @count int;
	declare @deleteOK bit = 0;
	declare @parentId int;
	declare @audienceId int
	
	-- get the parent and audience id of the record
	set @sql = 'select @parentid = [parentid], @audienceid = [audienceid] from [' + @tablename + '] where [id] = @Id';
	set @sqlParms = '@Id int, @parentid int OUTPUT, @audienceid int OUTPUT';
	exec sp_executesql @sql, @sqlParms, @Id = @audienceRecordId, @parentid = @parentId output, @audienceid = @audienceId output;
		
	
	-- first check to see if this is the only audience record. If it is, then the delete can happen.
	set @sql = 'select @retCount = count([id]) from [' + @tablename + '] where [parentID] = @pId';
	set @sqlParms = '@pId int, @retCount int OUTPUT';
	exec sp_executesql @sql, @sqlParms, @pId = @parentId, @retCount = @count output;	
	if @count = 1
		set @deleteOK = 1;	
	else
		begin
			-- if not, then we need to check that there are 1 or more audience records that have view and edit rights, then we can delete
			set @sql = 'select @retCount = count([id]) from [' + @tablename + '] where [canedit] = 1 and [canview] = 1 and [candelete] = 1 and [audienceid] <> @audId and [parentID] = @pId';
            set @sqlParms = '@audId int, @pId int, @retCount int OUTPUT';
			exec sp_executesql @sql, @sqlParms, @audId = @audienceId, @pId = @parentId, @retCount = @count output;
			
			if @count > 0
				set @deleteOK = 1;
		end
		
	if @deleteOK = 1
	begin
		set @sql = 'delete from [' + @tablename + '] where [id] = @recId;';
		set @sqlParms = '@recId int'

		exec sp_executesql @sql, @sqlParms, @recId = @audienceRecordId;

		exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @audienceRecordId, @tablename, null;
	end
	
	return @deleteOK	
end
