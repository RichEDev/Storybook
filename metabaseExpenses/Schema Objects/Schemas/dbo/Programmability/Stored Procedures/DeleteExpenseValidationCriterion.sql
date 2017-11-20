CREATE PROCEDURE [dbo].[DeleteExpenseValidationCriterion]
	@id int
AS
BEGIN
	SET NOCOUNT ON;

	-- check it even exists
	IF NOT EXISTS (SELECT CriterionId FROM [dbo].[ExpenseValidationCriteria] WHERE CriterionId = @id) RETURN -1;

	-- loop through each client db and check for anything that may be using the reason with this Id.	
	DECLARE @sql NVARCHAR(MAX);
	DECLARE @accountId INT;
	DECLARE @hostname NVARCHAR(100);
	DECLARE @dbName NVARCHAR(100);
	DECLARE @inUse BIT = 0;
	DECLARE @checkOUT INT;
	
	DECLARE accounts_cursor CURSOR FAST_FORWARD FOR 
		SELECT r.accountID, d.hostName, r.dbname
		FROM [dbo].[registeredusers] AS r 
		INNER JOIN [dbo].[databases] AS d ON d.databaseID = r.dbserver 
		WHERE r.archived = 0;

	OPEN accounts_cursor;

	FETCH NEXT FROM accounts_cursor INTO @accountId, @hostname, @dbName;
	WHILE @@fetch_status = 0
		BEGIN    
			SET @sql = 'SELECT @checkOUT = COUNT(CriterionId) FROM ' + @dbName + '.[dbo].[ExpenseValidationResults] WHERE CriterionId = ' + CAST(@id as NVARCHAR);
			EXEC sp_executesql @sql, N'@checkOUT int OUTPUT', @checkOUT OUTPUT;
			IF (@checkOUT > 0) SET @inUse = 1;

			FETCH NEXT FROM accounts_cursor INTO @accountId, @hostname, @dbName;
		END;
	CLOSE accounts_cursor;
	DEALLOCATE accounts_cursor;

	-- dip out if it is in use
	IF (@inUse = 1) RETURN -2;

	-- delete from metabase
	DELETE FROM [dbo].[ExpenseValidationCriteria]
	WHERE CriterionId = @id;
	RETURN 0;
END