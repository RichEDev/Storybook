CREATE PROCEDURE [dbo].[refreshSystemViews]
	@entityid INT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- if a field has been deleted, this can invalidate the systemviews, if there are any - so refresh each (can't tell which will be affected!)
	declare @sql nvarchar(250);
	declare @viewName nvarchar(250);
	declare @systemviewname nvarchar(250);

	declare loop_cursor cursor for
	select masterTableName from [customEntities] where systemview=1 and systemview_derivedentityid = @entityid
	open loop_cursor
	fetch next from loop_cursor into @systemviewname
	while @@FETCH_STATUS = 0
	begin
		set @viewName = 'custom_' + @systemviewname;
			
		set @sql = 'exec sp_refreshview ' + char(39) + @viewName + char(39);
			
		if exists (select * from sys.views where type = 'V' and name = @viewName)
		begin
			exec sp_executesql @sql
		end
				
		fetch next from loop_cursor into @systemviewname
	end
	close loop_cursor
	deallocate loop_cursor

RETURN 0