CREATE PROCEDURE [dbo].[DeleteClaimApproverDetailExpenceItems]
	@CheckerId INT = NULL,
	@SavedExpenseIds ExpencesItems READONLY

AS
BEGIN

DECLARE @ExpenseId  varchar(1000)
 DECLARE db_cursor
 CURSOR FOR  
SELECT
   SavedExpencesId
FROM
   @SavedExpenseIds


OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @ExpenseId  

WHILE @@FETCH_STATUS = 0  
BEGIN  
	IF EXISTS (SELECT * FROM ClaimApproverDetails c WHERE c.CheckerId = @CheckerId AND c.SavedExpenseId=@ExpenseId ) 
	BEGIN

	DELETE ClaimApproverDetails WHERE SavedExpenseId=@ExpenseId AND CheckerId = @CheckerId
	
	END
	 FETCH NEXT FROM db_cursor INTO @ExpenseId 
End

CLOSE db_cursor  
DEALLOCATE db_cursor 
RETURN @@RowCount
END