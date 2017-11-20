CREATE PROCEDURE [dbo].[deleteCustomEntity] (@entityid INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @pluralname NVARCHAR(250);
	SELECT @pluralname = plural_name FROM custom_entities WHERE entityid = @entityid;
	
	declare @attributeid int
	declare loop_cursor CURSOR FOR
	select attributeid from custom_entity_attributes where entityid = @entityid
	open loop_cursor
	fetch next from loop_cursor into @attributeid
	while(@@FETCH_STATUS = 0)
	begin
		exec dbo.deleteCustomAttribute @attributeid, @CUemployeeID, @CUdelegateID
		
		fetch next from loop_cursor into @attributeid
	end
	close loop_cursor
	deallocate loop_cursor

	DECLARE @sql NVARCHAR(4000)
	SET @sql = 'drop table [custom_' + replace(@pluralname,' ','_') + ']';
	EXECUTE sp_executesql @sql;
		
	if exists (SELECT TABLE_NAME
      FROM INFORMATION_SCHEMA.TABLES
      WHERE TABLE_NAME = 'custom_' + replace(@pluralname,' ','_') + '_attachmentData')
    BEGIN
		SET @sql = 'drop table [custom_' + replace(@pluralname,' ','_') + '_attachments]';
		EXECUTE sp_executesql @sql;
		
		SET @sql = 'drop table [custom_' + replace(@pluralname,' ','_') + '_attachmentData]';
		EXECUTE sp_executesql @sql;
	END
	------------update custom_entity_attributes set formid = null where formid in (select formid from custom_entity_forms where entityid = @entityid or entityid in (select entityid from custom_entity_attributes where related_entity = @entityid))

	delete from custom_entity_form_fields where formid in (select formid from custom_entity_forms where entityid = @entityid)
	delete from custom_entity_form_sections where formid in (select formid from custom_entity_forms where entityid = @entityid)
	delete from custom_entity_form_tabs where formid in (select formid from custom_entity_forms where entityid = @entityid)
	delete from custom_entity_forms where entityid = @entityid

	------------update custom_entity_attributes set viewid = null where viewid in (select viewid from custom_entity_views where entityid = @entityid or entityid in (select entityid from custom_entity_attributes where related_entity = @entityid))
	delete from custom_entity_view_filters where viewid in (select viewid from custom_entity_views where entityid = @entityid or entityid in (select entityid from custom_entity_attributes where related_entity = @entityid))
	delete from custom_entity_view_fields where viewid in (select viewid from custom_entity_views where entityid = @entityid)
	delete from custom_entity_views where entityid in (select viewid from custom_entity_views where entityid = @entityid)
	
	----------delete from custom_entity_attributes where entityid = @entityid or related_entity = @entityid

	delete FROM custom_entities WHERE entityid = @entityid;
END
