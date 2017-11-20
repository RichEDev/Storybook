CREATE function dbo.CheckAudienceAccess (
@tableid uniqueidentifier,
@recordId int,
@employeeId int
)
returns bit
as
begin
	declare @retVal bit
	set @retVal = 1; -- by default, allow access

	declare @tablename nvarchar(200)
	set @tablename = (select tablename from tables where tableid = @tableid)

	declare @sql nvarchar(max)
	declare @sqlParms nvarchar(150)
	declare @count int

	set @sql = 'select @retCount = count([id]) from [' + @tablename + ']'
	set @sqlParms = '@retCount int output';
	exec sp_executesql @sql, @sqlParms, @retCount = @count output;

	if @count > 0
	begin
		-- has an audience, so check employee is a member
		set @sql = 'select @retCount = count([id]) from [' + @tablename + '] where dbo.AudienceMemberCount([audienceID], @empid) > 0';
		set @sqlParms = '@empId int, @retCount int output';

		exec sp_executesql @sql, @sqlParms, @empId = @employeeId, @retCount = @count output

		if @count = 0
		begin
			set @retVal = 0
		end
	end

	return @retVal;	-- if result is zero, access will be denied
end
