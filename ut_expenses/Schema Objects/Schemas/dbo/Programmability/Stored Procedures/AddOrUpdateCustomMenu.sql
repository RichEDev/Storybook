
CREATE PROC AddOrUpdateCustomMenu
@custommenuid INT,
@name NVARCHAR(100),
@description NVARCHAR(500),
@parentmenuid INT,
@menuicon NVARCHAR(200),
@createdby INT,
@modifiedby INT,
@orderby INT

AS
BEGIN
SET NOCOUNT ON;

IF @custommenuid =0
BEGIN

INSERT INTO CustomMenu(Name,[Description],ParentMenuId,MenuIcon,CreatedBy,CreatedOn,OrderBy,SystemMenu) VALUES(@name,@description,@parentmenuid,@menuicon,@createdby,GETUTCDATE(),@orderby,0)
RETURN SCOPE_IDENTITY();
END
ELSE  IF NOT EXISTS (SELECT * FROM  CustomMenu WHERE SystemMenu = 1 AND CustomMenuId = @custommenuid)

BEGIN
	UPDATE CustomMenu 
	SET 
	Name = @name , 
	[Description] = @description,
	ParentMenuId=@parentmenuid ,
	MenuIcon=@menuicon,
	ModifiedBy=@modifiedby,
	ModifiedOn=GETUTCDATE(),
	OrderBy = @orderby
	WHERE CustomMenuId = @custommenuid 
	RETURN @custommenuid
	END
		RETURN 0;
	END