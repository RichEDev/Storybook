Create PROCEDURE [dbo].[SaveClaimApproverDetails]
	@ClaimId int,
	@ClaimantId int,
	@CheckerId int,
	@CreatedOn datetime,
	@SavedExpenseIds ExpencesItems READONLY

AS
BEGIN

Declare @ExpenseId  varchar(1000)
Declare @ClaimAmount decimal(18, 2)
set @ClaimAmount=0
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
	if Not EXISTS (SELECT * FROM ClaimApproverDetails c where c.ClaimId = @ClaimId and c.CheckerId=@CheckerId and c.SavedExpenseId=@ExpenseId) 
	begin

	set @ClaimAmount=( select total from savedexpenses where expenseid= @ExpenseId)
	    insert into ClaimApproverDetails (ClaimantId,CheckerId,CreatedOn,ClaimAmount,ClaimId,SavedExpenseId) values (@ClaimantId, @CheckerId,@CreatedOn,@ClaimAmount,@ClaimId, @ExpenseId)
	end
	 FETCH NEXT FROM db_cursor INTO @ExpenseId 
End

CLOSE db_cursor  
DEALLOCATE db_cursor 
return @ClaimantId
END
