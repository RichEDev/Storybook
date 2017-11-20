

CREATE PROCEDURE [dbo].[GetExpenseItemCompanyAddressMergeCount] 
	@mergeIDs nvarchar(max)
AS
BEGIN
DECLARE @Count int
DECLARE @ID int
DECLARE @Value nvarchar(10)
SET @Count = 0;
DECLARE loop_cursor CURSOR FOR
	SELECT Value FROM dbo.UTILfn_split(@mergeIDs, ',')
	OPEN loop_cursor
	FETCH NEXT FROM loop_cursor INTO @Value
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @ID = CAST(@Value AS int);
		SET @Count = @Count + (SELECT count(expenseid) FROM savedexpenses WHERE companyid = @ID);
		
		FETCH NEXT FROM loop_cursor INTO @Value
	END
	CLOSE loop_cursor
	DEALLOCATE loop_cursor

END
RETURN @Count;

