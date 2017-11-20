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
	SET @tmpCount = (select count(defaultCurrencyId) from customEntities where defaultCurrencyID = @currencyid);
	IF @tmpCount > 0
		RETURN 6;
  
    SET @tmpCount = (select count(CurrencyId) from bankaccounts where [CurrencyId] = @currencyid);
	IF @tmpCount > 0
		RETURN 9;
			
	BEGIN TRY			
		DECLARE @sql NVARCHAR(MAX);
		DECLARE @entityid INT;
		DECLARE @cnt INT;
		DECLARE @parmdef NVARCHAR(MAX);
		SET @parmdef = N'@out int OUTPUT';
		DECLARE DatabaseCursor CURSOR FAST_FORWARD FOR SELECT entityid FROM customEntities
		OPEN DatabaseCursor
		FETCH NEXT FROM DatabaseCursor INTO @entityid
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @sql = N'IF EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = ''custom_' + CAST(@entityid AS nvarchar) + ''' AND COLUMN_NAME = ''GreenLightCurrency'') BEGIN SET @out = 1 END ELSE BEGIN SET @out = 0 END';
			EXEC sp_executesql @sql, @parmdef, @out = @cnt OUTPUT
			
			IF @cnt > 0
			BEGIN
				-- Can only do this IF the GreenLightCurrency field exists
				SET @sql = 'SELECT @out = COUNT(*) FROM [custom_' + CAST(@entityid AS nvarchar) + N'] WHERE GreenLightCurrency = ' + CAST(@currencyid AS NVARCHAR(MAX));
				EXEC sp_executesql @sql, @parmdef, @out = @cnt OUTPUT
			
				IF @cnt > 0
				BEGIN
					SET @tmpCount = 1;
					BREAK;
				END
			END
			FETCH NEXT FROM DatabaseCursor INTO @entityid;
		END
		CLOSE DatabaseCursor
		DEALLOCATE DatabaseCursor 
	END TRY
	BEGIN CATCH
		CLOSE DatabaseCursor
		DEALLOCATE DatabaseCursor
		RETURN 8;
	END CATCH
	
	IF @tmpCount > 0
		RETURN 6; 
		
	SET @tmpCount = (SELECT count(currencyId) FROM mobileExpenseItems WHERE currencyid = @currencyid)
	IF @tmpCount > 0
		RETURN 7;
	
	SET @subAccountId = (select subAccountId from currencies where currencyid = @currencyid);
	UPDATE employees set primarycurrency = null WHERE primarycurrency = @currencyid;
	DELETE FROM currencies WHERE currencyid = @currencyid;
	SET @currency = (SELECT label FROM global_currencies WHERE globalcurrencyid = @globalcurrencyid);
	EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencyid, @currency, @subAccountId;
	RETURN 0;
END
