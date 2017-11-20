CREATE PROCEDURE [dbo].[customEntityAttributeIsDataUnique] 
@entityid int,
@attributeid int
AS
BEGIN
SET NOCOUNT ON;
DECLARE @tablename nvarchar(200);
DECLARE @sql nvarchar(1000);
SELECT @tablename = masterTableName FROM customEntities WHERE entityid = @entityid;
SET @sql = 'DECLARE @count int = null; SET @count = (SELECT TOP 1 COUNT(*) AS TotalCount FROM custom_' + CAST(@tablename AS nvarchar(500)) + ' WHERE att' + CAST(@attributeid AS nvarchar(15)) + ' IS NOT NULL GROUP BY att' + CAST(@attributeid AS nvarchar(15)) + ' HAVING COUNT(*) > 1); IF @count is null select 0; ELSE select @count;'
EXEC(@sql);
END