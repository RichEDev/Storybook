
CREATE PROC GetAllCustomMenu
AS
SET NOCOUNT ON;
BEGIN
SELECT CustomMenuId,Name,[Description],ParentMenuId,MenuIcon,OrderBy,SystemMenu FROM CustomMenu order by ParentMenuId , OrderBy asc
END