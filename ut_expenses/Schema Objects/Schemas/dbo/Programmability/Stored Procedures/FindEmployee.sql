CREATE PROCEDURE [dbo].[FindEmployee]
	@fieldId uniqueidentifier,
	@find nvarchar(4000)
AS
	declare @sql nvarchar(4000);
	select
		@sql = 'select
					source.employeeid
				from
					[' + [tables].tablename + '] source
						join employees e on e.employeeid = source.employeeid
				where
					source.[' + [fields].field + '] = @find
					and e.archived = 0
					and e.active = 1
					and e.locked = 0
				group by
					source.employeeid'
	from
		[fields]
			join [tables] on [tables].tableid = [fields].tableid
	where
		fields.fieldid = @fieldId

	exec sp_executesql @sql, N'@find nvarchar(4000)', @find = @find

RETURN 0
