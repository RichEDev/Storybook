CREATE PROCEDURE [dbo].[CheckIfAudienceIdIsUsed] 
	-- Add the parameters for the stored procedure here
	@audienceId int

AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @tablename nvarchar(500);
	DECLARE @foundcount int;
	DECLARE @sql nvarchar(500);
	DECLARE @sqlparams nvarchar(100);

	DECLARE audience_cursor CURSOR FOR 
	SELECT name FROM sys.objects WHERE charindex('_audiences', name) > 0 and TYPE='U'

	OPEN audience_cursor;

	FETCH NEXT FROM audience_cursor INTO @tablename;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @sql = 'select @tmp = count(audienceid) from [' + @tablename + '] where audienceid = @audId';
		SET @sqlparams = '@audId int, @tmp int output';
		EXEC sp_executesql @sql, @sqlparams, @audId = @audienceId, @tmp=@foundcount OUTPUT;
		IF @foundcount > 0
			GOTO ENDCURSOR;
		FETCH NEXT FROM audience_cursor INTO @tablename;

	END
	ENDCURSOR:
	CLOSE audience_cursor;
	DEALLOCATE audience_cursor;

	IF @foundcount > 0
		RETURN 1;
	ELSE
		RETURN -1;
END
