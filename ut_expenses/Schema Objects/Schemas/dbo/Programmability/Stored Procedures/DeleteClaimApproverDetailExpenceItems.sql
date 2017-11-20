Create PROCEDURE [dbo].[DeleteClaimApproverDetailExpenceItems]
	@CheckerId int,
	@SavedExpenseIds ExpencesItems READONLY

AS
BEGIN

Declare @ExpenseId  varchar(1000)
 dECLARE db_cursor
 CURSOR FOR  
SELECT
   SavedExpencesId
FROM
   @SavedExpenseIds


OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @ExpenseId  

WHILE @@FETCH_STATUS = 0  
BEGIN  
	if EXISTS (SELECT * FROM ClaimApproverDetails c where c.CheckerId = @CheckerId and c.SavedExpenseId=@ExpenseId ) 
	begin

	delete ClaimApproverDetails where SavedExpenseId=@ExpenseId and CheckerId = @CheckerId
	
	end
	 FETCH NEXT FROM db_cursor INTO @ExpenseId 
End

CLOSE db_cursor  
DEALLOCATE db_cursor 
return @@RowCount
END