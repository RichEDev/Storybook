CREATE PROCEDURE [dbo].[unmatchTransaction] 
	@expenseId INT
AS
BEGIN
 
 SET NOCOUNT ON;

 
    update dbo.savedexpenses set transactionid = null where expenseid = @expenseid or expenseid in (select splititem from dbo.savedexpensessplithierarchy where primaryitem =@expenseid)
END

GO