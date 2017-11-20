
CREATE PROCEDURE [dbo].[deleteCustomAttribute] (@attributeid INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 DECLARE @masterTableName NVARCHAR(250);
 declare @systemviewname nvarchar(250);
 DECLARE @entityid INT;
 DECLARE @attributename NVARCHAR(100);
 DECLARE @fieldtype TINYINT
 declare @relationshiptype tinyint
 DECLARE @relatedtableid UNIQUEIDENTIFIER;
 declare @fieldid uniqueidentifier
 DECLARE @relatedtable NVARCHAR(250);
 DECLARE @isKeyField bit = 0;
 DECLARE @triggerJoinViaID int;
 SELECT @entityid = entityid, @attributename = display_name, @fieldtype = fieldtype, @fieldid = fieldid, @relationshiptype = relationshiptype, @relatedtableid = relatedtable, @isKeyField = is_key_field, @triggerJoinViaID = TriggerJoinViaId FROM [customEntityAttributes] WHERE attributeid = @attributeid; 
 
 -- we can't delete the primary key attribute as it will error with constraint issues
 if @attributename = 'ID' AND @isKeyField = 1
 begin
  return -3;
 end

 IF EXISTS(SELECT attributeid FROM customEntityAttributes WHERE attributeid = @attributeid AND BuiltIn = 1)
 BEGIN
	RETURN -9;
 END
 
 if @attributename = 'Created By' or @attributename = 'Modified By' or @attributename = 'Created On' or @attributename = 'Modified On' 
 begin
  SET @attributename = REPLACE(@attributename, ' ', '');
 end
 
 if @attributename = 'GreenLight Currency'
 begin
  set @attributename = 'GreenLightCurrency';
 end
 
 declare @entityTableId uniqueidentifier = (select tableid from [customEntities] where entityid = @entityid);
 SELECT @masterTableName = masterTableName FROM [customEntities] WHERE entityid = @entityid;
 select @relatedtable = tablename from tables where tableid = @relatedtableid;
 
 DECLARE @sql NVARCHAR(4000)

 declare @table_name nvarchar(250)
 set @table_name = 'custom_' + @masterTableName;

 -- Check that attribute is not used for lookup or as a display field by a n:1 attribute
 if exists (select relationshipDisplayField from customEntityAttributes where relationshipDisplayField = @fieldid OR TriggerDisplayFieldId = @fieldid)
 begin
  return -1;
 end

 if exists (select fieldId from customEntityAttributeMatchFields where fieldId = @fieldid)
 begin
  return -2;
 end

  if exists (select displayField from userdefined where displayField = @fieldid)
 begin
  return -4;
 end
 
 if exists (select fieldId from userdefinedMatchFields where fieldId = @fieldid)
 begin
  return -5;
 end

 IF EXISTS(SELECT DISTINCT joinViaID FROM joinViaParts WHERE relatedID = @fieldid and joinViaID <> (select triggerJoinViaID from customEntityAttributes where fieldid = @fieldid))
 BEGIN
  -- if joinVia is not used elsewhere, then delete the joinvia
  declare @jvRetId int;

  exec @jvRetId = dbo.DeleteJoinViasUsingRelatedID @fieldid;
  if @jvRetId <> 0
  begin
   RETURN -6;
  end
 END

 IF EXISTS (SELECT * FROM reportcolumns WHERE FIELDID = @fieldid)
 begin
	RETURN -7;
 end

 IF EXISTS (SELECT * FROM reportcriteria WHERE fieldID = @fieldid)
 begin
	RETURN -7;
 end

 IF EXISTS (SELECT * FROM customEntities WHERE entityid = @entityid AND formSelectionAttribute = @attributeid)
 begin
	RETURN -8;
 end

 IF @fieldtype = 9
 BEGIN
  declare @constraint_name nvarchar(250)
  
  if @relationshiptype = 2
  begin
   delete from [customEntityAttributeSummary] where otm_attributeid = @attributeid;
   
   set @constraint_name = 'FK_' + @table_name + '_' + @relatedtable + '_' + cast(@attributeid as nvarchar(10))
  
   IF EXISTS (SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE TABLE_NAME = @relatedtable AND CONSTRAINT_NAME = @constraint_name)
   BEGIN
    SET @sql = 'ALTER TABLE [' + @relatedtable + '] DROP [' + @constraint_name + ']';
    EXECUTE sp_executesql @sql
   END

   -- need to delete any records that belong to this otm relationship in the related table
   set @sql = 'delete from [' + @relatedtable + '] where [att' + cast(@attributeid as nvarchar) + '] is not null';
   EXECUTE sp_executesql @sql

   declare @related_entityid int;
   declare @systemview_entityid int;
   declare @systemview_tableid uniqueidentifier;
   declare @systemview_jointableid uniqueidentifier;
   
   select @related_entityid = related_entity from [customEntityAttributes] where attributeid = @attributeid;
   select @systemviewname = masterTableName, @systemview_entityid = entityid, @systemview_tableid = tableid from [customEntities] where systemview_entityid = @entityid and systemview_derivedentityid = @related_entityid;

   declare loop_cursor cursor for
   select jointableid from [customJoinTables] where tableid = @systemview_tableid or basetableid = @systemview_tableid
   open loop_cursor
   fetch next from loop_cursor into @systemview_jointableid
   while @@FETCH_STATUS = 0
   begin
    delete from [customJoinBreakdown] where jointableid = @systemview_jointableid;
    delete from [customJoinTables] where jointableid = @systemview_jointableid;
   
    fetch next from loop_cursor into @systemview_jointableid
   end
   close loop_cursor
   deallocate loop_cursor;

   declare @jtId uniqueidentifier;
   declare loop_cursor cursor for
   select jointableid from [customJoinTables] where tableid = @relatedtableid and basetableid = @entityTableId
   open loop_cursor
   fetch next from loop_cursor into @jtId
   while @@FETCH_STATUS = 0
   begin
    delete from [customJoinBreakdown] where jointableid = @jtId;
    delete from [customJoinTables] where jointableid = @jtId;
    
    fetch next from loop_cursor into @jtId
   end
   close loop_cursor
   deallocate loop_cursor;

   if exists (select * from information_schema.tables where table_type = 'VIEW' and table_name = 'custom_' + @systemviewname)
   begin
    SET @sql = 'drop view [custom_' + @systemviewname + ']';
    EXECUTE sp_executesql @sql;
   end

   delete from [customViewGroups] where viewgroupid = (select distinct viewgroupid from [customFields] where tableid = @systemview_tableid);
   delete from [customFields] where tableid = @systemview_tableid;
   delete from [customEntities] where tableid = @systemview_tableid;

   SET @sql = 'if exists (select * from information_schema.columns where table_name = ''' + @relatedtable + ''' and column_name = ''att' + cast(@attributeid as nvarchar(10)) + ''') alter table [' + @relatedtable + '] drop column [att' + cast(@attributeid as nvarchar(10)) + ']';
   EXECUTE sp_executesql @sql;
  end
  ELSE
  BEGIN
   if @attributename = 'CreatedBy' or @attributename = 'ModifiedBy' -- reserved n:1 fields
   begin
    if exists (select * from sys.foreign_keys where type = 'F' and name = 'FK_' + @table_name + '_' + @attributename + '_employees')
    begin
     set @sql = 'ALTER TABLE [dbo].[' + @table_name + '] DROP CONSTRAINT [FK_' + @table_name + '_' + @attributename + '_employees]';
     exec sp_executesql @sql;
    end

    SET @sql = 'if exists (select * from information_schema.columns where table_name = ''' + @table_name + ''' and column_name = ''' + @attributename + ''') alter table [' + @table_name + '] drop column [' + @attributename + ']';
   end
   else
   begin    
    delete from [customJoinTables] where basetableid = @entityTableId and tableid = @relatedtableid;

    -- drop foreign key if it exists
    SET @constraint_name = 'FK_' + @table_name + '_' + @relatedtable + '_' + cast(@attributeid as nvarchar);
    IF EXISTS (SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE TABLE_NAME = @table_name AND CONSTRAINT_NAME = @constraint_name)
    BEGIN
     SET @sql = 'ALTER TABLE [' + @table_name + '] DROP [' + @constraint_name + ']';
     EXECUTE sp_executesql @sql
    END

    SET @sql = 'if exists (select * from information_schema.columns where table_name = ''' + @table_name + ''' and column_name = ''att' + cast(@attributeid as nvarchar(10)) + ''') alter table [' + @table_name + '] drop column [att' + cast(@attributeid as nvarchar(10)) + ']';
   end
   EXECUTE sp_executesql @sql;

   EXECUTE DeleteJoinViasUsingRelatedID @fieldid
  END
 END
 ELSE
 BEGIN
  if @fieldtype not in (11, 19, 21)
  begin
   if @attributename = 'CreatedOn' or @attributename = 'ModifiedOn' or @attributename = 'GreenLightCurrency' -- reserved entity attributes
   begin
    if @attributename = 'GreenLightCurrency'
    begin
     declare @EntityCurrencyFKName nvarchar(300) = 'FK_' + @masterTableName + '_currencies';
     set @sql = 'IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N''[dbo].[' + @EntityCurrencyFKName + ']'') AND parent_object_id = OBJECT_ID(N''[dbo].[' + @table_name + ']'')) ALTER TABLE [dbo].[' + @table_name + '] DROP CONSTRAINT [' + @EntityCurrencyFKName + ']';
     exec sp_executesql @sql;
    end	

    SET @sql = 'if exists (select * from information_schema.columns where table_name = ''' + @table_name + ''' and column_name = ''' + @attributename + ''') alter table [' + @table_name + '] drop column [' + @attributename + ']';
   end
   else
   begin
    SET @sql = 'if exists (select * from information_schema.columns where table_name = ''' + @table_name + ''' and column_name = ''att' + cast(@attributeid as nvarchar(10)) + ''') alter table [' + @table_name + '] drop column [att' + cast(@attributeid as nvarchar(10)) + ']';
   end

   EXECUTE sp_executesql @sql;
  END
 END

 EXECUTE dbo.refreshSystemViews @entityid;
 
 -- Delete any trigger attributes linked to this attribute
 if @fieldtype <> 21
 begin
  declare @lookupDisplayAttributeId int;
  declare trigger_loop cursor for
  select attributeid from customEntityAttributes where triggerAttributeId = @attributeid
  open trigger_loop
  fetch next from trigger_loop into @lookupDisplayAttributeId
  while @@FETCH_STATUS = 0
  begin
   exec dbo.deleteCustomAttribute @lookupDisplayAttributeId, @CUemployeeID, @CUdelegateID;

   fetch next from trigger_loop into @lookupDisplayAttributeId
  end
  close trigger_loop
  deallocate trigger_loop
 end
 ELSE 
	--delete Joinvia details if the joinvia is not used on any other custom entity attribute.
	 DECLARE @rowCount int
	 select @rowCount = count(triggerJoinViaID) from customEntityAttributes where TriggerJoinViaId = @triggerJoinViaID
	  If @rowCount <= 1
		BEGIN
			Delete joinViaParts where joinViaID = @triggerJoinViaID
			Delete joinVia where joinViaID = @triggerJoinViaID
		END
 
if @fieldtype = 22
begin
Delete from customJoinBreakdown where sourcetable = @entityTableId and destinationKey = @fieldid
End

 delete from [customEntityAttributeSummary] where attributeid = @attributeid;
 delete from [customEntityAttributeSummaryColumns] where attributeid = @attributeid;
 delete from [customEntityAttributeListItems] where attributeid = @attributeid;

 delete from [customEntityFormFields] where attributeid = @attributeid;
 delete from [customEntityViewFields] where fieldid = (select fieldid from [customEntityAttributes] where attributeid = @attributeid);
 delete from [fieldFilters] where attributeid = @attributeid;

 delete from [customFields] where field = 'att' + cast(@attributeid as nvarchar);

 -- if this is the audit identifier for the entity, then reset it to the id attribute
 if exists(select * from customentityattributes where attributeid=@attributeid and is_audit_identity = 1)
  update customentityattributes set is_audit_identity = 1, CacheExpiry = GETUTCDATE() where display_name = 'id' and entityid = @entityid;

 DELETE fieldFilters WHERE [value]=@attributeid AND isParentFilter=1
 DELETE FROM [customEntityAttributes] WHERE attributeid = @attributeid;

 return 0;
END