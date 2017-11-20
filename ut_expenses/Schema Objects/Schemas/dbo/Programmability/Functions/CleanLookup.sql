create function [dbo].[CleanLookup] 
(@lookup nvarchar(256))
returns nvarchar(256)
as
begin

declare @firstCharPos int = patindex('%[A-Za-z]%', @lookup)
if @firstCharPos = 0 return ''
return isnull(replace(upper(substring(@lookup, @firstCharPos, len(@lookup))), ' ', ''), '')

end