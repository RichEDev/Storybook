CREATE PROCEDURE unmatchTransaction 
 @expenseId INT
AS
BEGIN

 SET NOCOUNT ON;

 DECLARE @ids intpk
 insert into @ids (c1) values (@expenseId)

    update savedexpenses set transactionid = null where expenseid in (select c1 from dbo.getExpenseItemIdsFromPrimary(@ids))
END