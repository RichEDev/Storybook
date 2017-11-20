CREATE PROCEDURE [dbo].[deleteUserdefined] 
	-- Add the parameters for the stored procedure here
	@userdefineid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	DECLARE @recordTitle nvarchar(2000);
	DECLARE @ViewFieldCount int;
	DECLARE @ViewFilterCount int;
	DECLARE @FieldID uniqueidentifier;
	
	SELECT @recordTitle = attribute_name, @FieldID = fieldid  FROM userdefined WHERE userdefineid = @userdefineid
	
	SET @ViewFieldCount = (SELECT count(fieldid) FROM customEntityViewFields WHERE fieldID = @FieldID);
	IF @ViewFieldCount > 0
	BEGIN
		RETURN -1
	END
	
	SET @ViewFilterCount = (SELECT count(fieldid) FROM [fieldFilters] WHERE fieldID = @FieldID);
	IF @ViewFilterCount > 0
	BEGIN
		RETURN -2;
	END

	-- Check that attribute is not used for lookup or as a display field by a n:1 attribute
	if exists (select relationshipDisplayField from customEntityAttributes where relationshipDisplayField = @FieldID OR TriggerDisplayFieldId = @FieldID)
	begin
		return -3;
	end

	if exists (select fieldId from customEntityAttributeMatchFields where fieldId = @FieldID)
	begin
		return -4;
	end

	 if exists (select displayField from userdefined where displayField = @FieldID)
	begin
		return -5;
	end

	if exists (select fieldId from userdefinedMatchFields where fieldId = @FieldID)
	begin
		return -6;
	end

	IF EXISTS(SELECT DISTINCT joinViaID FROM joinViaParts WHERE relatedID = @FieldID)
	BEGIN
		RETURN -7;
	END

    IF EXISTS (SELECT * FROM reportcolumns WHERE fieldId = @fieldid)
    BEGIN
		RETURN -8;
    END

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @tablename nvarchar(50);
	declare @fieldname nvarchar(100);
	declare @sql nvarchar(max);
    -- Insert statements for procedure here
    BEGIN TRANSACTION
		select @tablename = tablename from tables inner join userdefined on userdefined.tableid = tables.tableid where userdefined.userdefineid = @userdefineid;
		select @fieldname = field from fields inner join userdefined on userdefined.fieldid = fields.fieldid where userdefined.userdefineid = @userdefineid;
		set @sql = 'alter table [' + @tablename + '] drop column [' + @fieldname + ']';
		EXECUTE sp_executesql @sql
		delete from userdefined where userdefineid = @userdefineid;

		exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 50, @userdefineid, @recordTitle, null;
	
	IF @@ERROR <> 0
		BEGIN 
			ROLLBACK
			RETURN 0
		END
	COMMIT
		
	RETURN 1
END