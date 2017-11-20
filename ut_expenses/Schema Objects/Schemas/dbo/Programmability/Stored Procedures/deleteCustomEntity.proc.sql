CREATE PROCEDURE [dbo].[deleteCustomEntity] (@entityid INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @entityTableId uniqueidentifier;
	select @entityTableId = tableId from customEntities where entityid = @entityid;

	-- don't permit deletion of the custom entity if a 1:n relationship TO this entity exists
	if exists (select * from [customEntityAttributes] where relatedtable = @entityTableId and relationshipType = 2)
	begin
		return -1;
	end

	-- don't permit deletion of the custom entity if the entity doesn't exist
	if not exists (select * from [customEntities] where entityid = @entityid)
	begin
		return -2;
	end

	-- -3 error return code is permission denied

	-- don't permit deletion of the custom entity if a n:1 relationship references this entity
	if exists (select * from [customEntityAttributes] where relatedtable = @entityTableId and relationshipType = 1)
	begin
		return -4;
	end

	-- don't permit deletion of built-in (system) greenlights
	IF EXISTS (SELECT entityid FROM [customEntities] WHERE entityid = @entityid AND builtIn = 1)
	BEGIN
		RETURN -6;
	END

	BEGIN TRANSACTION [deleteGreenLight]

	BEGIN TRY
		-- Insert statements for procedure here
		DECLARE @masterTableName NVARCHAR(250);
		DECLARE @attRetCode int;
		SELECT @masterTableName = masterTableName FROM [customEntities] WHERE entityid = @entityid;

		declare @attributeid int
		declare entity_loop_cursor CURSOR FOR
		select attributeid from [customEntityAttributes] where entityid = @entityid and is_key_field = 0
		open entity_loop_cursor
		fetch next from entity_loop_cursor into @attributeid
		while @@FETCH_STATUS = 0
		begin
			begin try
				exec @attRetCode = dbo.deleteCustomAttribute @attributeid, @CUemployeeID, @CUdelegateID

				if @attRetCode < 0
				begin
					-- If return -1 or -2 then attribute used as display or lookup field in a n:1 (though this should be caught in -4 return code above)
					-- If return -3 then trying to delete the primary key (shouldn't happen due to the query used for this loop)
					close entity_loop_cursor
					deallocate entity_loop_cursor

					ROLLBACK TRANSACTION [deleteGreenLight];
					return -5;
				end
			end try
			begin catch

			end catch

			fetch next from entity_loop_cursor into @attributeid
		end
		close entity_loop_cursor
		deallocate entity_loop_cursor

		DECLARE @sql NVARCHAR(4000);

  if exists (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'custom_' + @masterTableName + '_attachments')
  BEGIN
   SET @sql = 'drop table [custom_' + @masterTableName + '_attachments]';
   EXECUTE sp_executesql @sql;
  END

  if exists (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'custom_' + @masterTableName + '_attachmentData')
  BEGIN
   SET @sql = 'drop table [custom_' + @masterTableName + '_attachmentData]';
   EXECUTE sp_executesql @sql;
  END

  if exists (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'custom_' + @masterTableName + '_audiences')
  BEGIN
   declare @audienceTableId uniqueidentifier;
   select @audienceTableId = tableid from tables where tablename = 'custom_' + @masterTableName + '_audiences';
   delete from [customFields] where tableid = @audienceTableId;

   delete from [customJoinTables] where basetableid = @audienceTableId;

   SET @sql = 'drop table [custom_' + @masterTableName + '_audiences]';
   EXECUTE sp_executesql @sql;
  END

  if exists (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'custom_' + @masterTableName AND TABLE_TYPE = 'BASE TABLE')
  BEGIN
   SET @sql = 'drop table [custom_' + @masterTableName + ']';
   EXECUTE sp_executesql @sql;
  END
 
  declare @viewgroupid uniqueidentifier = (select tableid from [customEntities] where entityid = @entityid);
  update [customViewGroups] set parentid = null where parentid = @viewgroupid;
  delete from [customViewGroups] where viewgroupid = @viewgroupid;

  ------------update customEntityAttributes set formid = null where formid in (select formid from custom_entity_forms where entityid = @entityid or entityid in (select entityid from customEntityAttributes where related_entity = @entityid))

  delete from [customEntityFormFields] where formid in (select formid from [customEntityForms] where entityid = @entityid)
  delete from [customEntityFormSections] where formid in (select formid from [customEntityForms] where entityid = @entityid)
  delete from [customEntityFormTabs] where formid in (select formid from [customEntityForms] where entityid = @entityid)
  delete from [customEntityForms] where entityid = @entityid

  ------------update customEntityAttributes set viewid = null where viewid in (select viewid from custom_entity_views where entityid = @entityid or entityid in (select entityid from customEntityAttributes where related_entity = @entityid))
  delete from [fieldFilters] where viewid in (select viewid from [customEntityViews] where entityid = @entityid or entityid in (select entityid from [customEntityAttributes] where related_entity = @entityid))
  delete from [customEntityViewFields] where viewid in (select viewid from [customEntityViews] where entityid = @entityid)
  delete from [customEntityViews] where entityid = @entityid
 
  ----------delete from customEntityAttributes where entityid = @entityid or related_entity = @entityid

  delete from [customEntities] where entityid = @entityid;
  
  delete from customJoinTables where tableid = @entityTableId;

  COMMIT TRANSACTION [deleteGreenLight];
 END TRY
 BEGIN CATCH
  ROLLBACK TRANSACTION [deleteGreenLight]
  return -99;
 END CATCH

 return 0;
END
