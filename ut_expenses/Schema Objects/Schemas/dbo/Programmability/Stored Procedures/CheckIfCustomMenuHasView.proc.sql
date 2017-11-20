Create PROC CheckIfCustomMenuHasView
@CustomMenuId int
AS
BEGIN
SET NOCOUNT ON;
IF EXISTS (SELECT * FROM customEntityViews WHERE menuid = @custommenuid)
BEGIN
RETURN 1
END
RETURN 0
END

GO