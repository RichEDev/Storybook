
CREATE PROCEDURE [dbo].[updateCacheExpiry] 
	@tablename nvarchar(300),
	@tablecolumn nvarchar(300),
	@id int

AS
BEGIN

DECLARE @sql nvarchar(2500)
DECLARE @paramDef nvarchar(50)

SET @paramDef = '@tempid int';

	SET @sql = 'UPDATE [' + @tablename + '] SET CacheExpiry = getDate() WHERE [' + @tablecolumn + '] = @tempid'

    EXECUTE sp_executesql @sql, @paramDef, @tempid = @id
	
END
