CREATE PROCEDURE [dbo].[SaveCustomEntityDisabledModuleMenuViews] 
	@menuId INT,
	@viewId INT, 
	@moduleIds IntPK READONLY
AS
BEGIN
	DELETE FROM CustomEntityDisabledModuleMenuView WHERE viewid = @viewid;

	INSERT INTO CustomEntityDisabledModuleMenuView (Menuid, ViewId, ModuleId) 
	SELECT @menuid, @viewid, c1 from @moduleIds;
 
	RETURN 0;
END