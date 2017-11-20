CREATE PROCEDURE [dbo].[GetTreeViewNodes] 
	@AssociatedID uniqueidentifier,
	@IsSubNode bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @IsSubNode = 0
		BEGIN
			SELECT DISTINCT viewgroups.viewgroupid, viewgroups.groupname, viewgroups.parentid, viewgroups.level from [viewgroups] where viewgroupid in (select viewgroupid from fields inner join reports_allowedtables on fields.tableid = reports_allowedtables.tableid where parentid is null and basetableid = @AssociatedID) order by [level], [groupname]
		END
	ELSE
		BEGIN
			SELECT DISTINCT viewgroups.viewgroupid, viewgroups.groupname, viewgroups.parentid, viewgroups.level from [viewgroups] where parentid = @AssociatedID order by [level], [groupname]
		END
END
