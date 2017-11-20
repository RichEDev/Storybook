create function [dbo].[IncrementLookup](@lookup nvarchar(256), @pos int  = null)
returns nvarchar(256)
as
begin
if ISNULL(@lookup,'') = '' return 'ZZZ'

if @pos is null select @pos = LEN(@lookup)
declare @retval nvarchar(256)
if SUBSTRING(@lookup, @pos, 1) = 'Z'
	IF @pos = 1 SELECT @retval = @lookup + 'ZZZ'  --shouldn't ever need this unless passed in ZZZ
	else select @retval = dbo.IncrementLookup(@lookup, @pos - 1)
else
	select @retval = substring(@lookup, 1, @pos - 1) + char(ascii(substring(@lookup, @pos, 1)) + 1)

return @retval
end
