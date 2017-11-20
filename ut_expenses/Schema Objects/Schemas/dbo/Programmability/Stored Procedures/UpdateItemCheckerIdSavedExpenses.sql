Create PROCEDURE [dbo].[UpdateItemCheckerIdSavedExpenses] 
@SavedExpenseIds ExpencesItems READONLY,
@CheckerId int

As
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
update savedexpenses set itemCheckerId=@CheckerId , tempallow=0 , IsItemEscalated=1 where expenseid=@ExpenseId
FETCH NEXT FROM db_cursor INTO @ExpenseId  
End

CLOSE db_cursor  
DEALLOCATE db_cursor 
END
