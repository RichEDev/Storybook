CREATE PROCEDURE [dbo].[deleteCurrency] 
	@currencyid int,
	@globalcurrencyid int,
	@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS

DECLARE @tmpCount int;
DECLARE @currency nvarchar(50);
DECLARE @subAccountId int;
BEGIN
	SET @tmpCount = (SELECT count(stringValue) FROM [accountProperties] WHERE stringKey='baseCurrency' AND stringValue = @currencyid)
	IF @tmpCount > 0
		RETURN 1;
	SET @tmpCount = (SELECT count(currencyid) FROM savedexpenses WHERE currencyid = @currencyid OR basecurrency = @currencyid OR globalbasecurrency = @currencyid)
	IF @tmpCount > 0
		RETURN 2;
	SET @tmpCount = (SELECT count(currencyid) FROM claims WHERE currencyid = @currencyid)
	IF @tmpCount > 0
		RETURN 2;
	SET @tmpCount = (SELECT count(contractCurrency) FROM contract_details WHERE contractCurrency = @currencyid)
	IF @tmpCount > 0
		RETURN 3;
	SET @tmpCount = (SELECT count(supplier_currency) FROM supplier_details WHERE supplier_currency = @currencyid)
	IF @tmpCount > 0
		RETURN 4;
	SET @tmpCount = (SELECT count(currencyId) FROM contract_productdetails WHERE currencyId = @currencyid)
	IF @tmpCount > 0
		RETURN 5;
	
	-- CUSTOM ENTITIES
	BEGIN TRY
		DECLARE @sql NVARCHAR(MAX);
		DECLARE @dbname NVARCHAR(MAX);
		DECLARE @cnt INT;
		DECLARE @parmdef NVARCHAR(MAX);
		SET @parmdef = N'@out int OUTPUT';

		DECLARE DatabaseCursor CURSOR FAST_FORWARD FOR SELECT plural_name FROM custom_entities
		OPEN DatabaseCursor
		FETCH NEXT FROM DatabaseCursor INTO @dbname

		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @sql = N'SELECT @out = COUNT(*) FROM [custom_' + REPLACE(@dbname, ' ', '_') + N'] WHERE EntityCurrency = ' + CAST(@currencyid AS NVARCHAR(MAX))
			EXEC sp_executesql @sql, @parmdef, @out = @cnt OUTPUT

			IF @cnt > 0
			BEGIN
				SET @tmpCount = 1
			END
			
			FETCH NEXT FROM DatabaseCursor INTO @dbname;
		END
		CLOSE DatabaseCursor
		DEALLOCATE DatabaseCursor		
	END TRY
	BEGIN CATCH
		CLOSE DatabaseCursor
		DEALLOCATE DatabaseCursor
	END CATCH
	
	IF @tmpCount > 0
		RETURN 6;	
		
	SET @subAccountId = (select subAccountId from currencies where currencyid = @currencyid);
	
	UPDATE employees set primarycurrency = null WHERE primarycurrency = @currencyid;
	DELETE FROM currencies WHERE currencyid = @currencyid;

	SET @currency = (SELECT label FROM global_currencies WHERE globalcurrencyid = @globalcurrencyid);
	EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencyid, @currency, @subAccountId;
	
	RETURN 0;
END
