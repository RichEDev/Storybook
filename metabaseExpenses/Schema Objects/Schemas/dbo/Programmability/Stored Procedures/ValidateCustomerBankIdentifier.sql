CREATE PROCEDURE [dbo].[ValidateCustomerBankIdentifier]
	@cardProviderId int,
	@FileIdentifier nvarchar(max)
AS

DECLARE @accountid INT;
DECLARE @dbname NVARCHAR(MAX);
DECLARE @sql NVARCHAR(MAX);
DECLARE AccountCursor CURSOR FOR SELECT accountid, dbname FROM registeredusers WHERE archived = 0;
DECLARE @result int= -1;
declare @cardProviderCount int;

OPEN AccountCursor;

FETCH NEXT FROM AccountCursor INTO
@accountid, @dbname

WHILE @@FETCH_STATUS = 0
BEGIN
	SET @sql = 'SELECT @cardProviderCountOUT =  COUNT(cardproviderid) FROM '+@dbname+'.dbo.corporate_cards WHERE cardproviderid = '+CAST(@cardProviderId as nvarchar(max))+' AND FileIdentifier = '''+@FileIdentifier+''''
	declare @ParmDefinition nvarchar(max) = N'@cardproviderCountOUT int OUTPUT'; 
	EXEC sp_executesql @sql, @ParmDefinition, @cardProviderCountOUT=@cardProviderCount OUTPUT;
	IF @cardProviderCount = 1
	BEGIN
		IF @result = -1
		BEGIN
			SET @result = @accountid;
		END
		ELSE
		BEGIN
			select -100; -- duplicate
		END
	END
	ELSE
	BEGIN
		IF @cardProviderCount > 1
		BEGIN
			select -100; -- Duplicate
		END
	END

	FETCH NEXT FROM AccountCursor INTO
	@accountid, @dbname
	
END
CLOSE AccountCursor
DEALLOCATE AccountCursor

select @result;

