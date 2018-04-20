CREATE PROCEDURE [dbo].[EncryptCustomEntityAttributeData]
	@attributeId INT,
	@tableId UNIQUEIDENTIFIER,
	@salt nvarchar(40)
AS
	
	DECLARE @tablename AS NVARCHAR(MAX) = (SELECT tablename FROM tables WHERE tableid = @tableId);
	DECLARE @fieldname AS NVARCHAR(MAX) = 'udf' + CAST(@attributeId AS NVARCHAR(50));

	IF NOT EXISTS(SELECT * FROM CustomerFields WHERE tableid = @tableId AND field = @fieldname)
	BEGIN
		SET @fieldname  = 'att' + CAST(@attributeId AS NVARCHAR(50));
	END

	DECLARE @sql AS NVARCHAR(MAX);

	SET @sql = 'ALTER TABLE ' + @tablename + ' ADD ' + @fieldname + '_encrypted VARBINARY(max); '
	EXEC( @sql);


	SET @sql = 'UPDATE '+@tablename+' SET '+@fieldname+'_encrypted =  ENCRYPTBYPASSPHRASE('''+@salt+''', '+@fieldname+') FROM '+@tablename+'; ';
	EXEC( @sql);

	SET @sql = 'ALTER TABLE '+@tablename+' DROP COLUMN '+@fieldname+' ;';

	EXEC(@sql);

	DECLARE @tableAndField AS NVARCHAR(MAX) = @tablename + '.'+@fieldname+'_encrypted';

	EXEC sp_rename @tableAndField, @fieldname, 'COLUMN';  

RETURN 0

