create function dbo.getCurrencySymbol
(
@charid nvarchar(50)
)
returns nvarchar
as
begin
declare @outputchar nvarchar(50)
set @outputchar = ''

if cast(charindex(',', @charid) as int) > 0
begin
	declare @idx_start int
	declare @idx_comma int
	declare @tmpchar nvarchar(50)
	declare @tmpoutstr nvarchar(50)
	
	set @tmpoutstr = ''
	set @idx_start = 1
	
	while @idx_start < len(@charid)
	begin
		set @idx_comma = cast(charindex(',', @charid, @idx_start) as int)
		
		if @idx_comma > 0
		begin
			set @tmpchar = substring(@charid,@idx_start,(@idx_comma-@idx_start))

			set @tmpoutstr = @outputchar + (SELECT CONVERT(NVARCHAR(50), NCHAR(LTRIM(RTRIM(@tmpchar)))))
			set @outputchar = @tmpoutstr	
			
			set @idx_start = @idx_comma + 1
		end
		else
		begin
			set @tmpchar = substring(@charid, @idx_start,(len(@charid)-@idx_comma))
			set @tmpoutstr = @outputchar + (SELECT CONVERT(NVARCHAR(50), NCHAR(LTRIM(RTRIM(@tmpchar)))))
			set @outputchar = @tmpoutstr
			set @idx_start = len(@charid)
		end
	end
end
else
begin
	set @outputchar = (SELECT CONVERT(NVARCHAR(50), NCHAR(@charid)));
end
return @outputchar
end