CREATE PROCEDURE [dbo].[GetExpenseItemsFromIds]
	@expenseids IntPK READONLY
AS
	
SELECT savedexpenses.*,dbo.float_allocations.floatid, dbo.returnedexpenses.note,  dbo.returnedexpenses.dispute, dbo.returnedexpenses.corrected 
 FROM  dbo.savedexpenses 
 INNER JOIN @expenseids AS source ON source.c1 = savedexpenses.expenseid
 LEFT OUTER JOIN  dbo.float_allocations ON dbo.savedexpenses.expenseid = dbo.float_allocations.expenseid 
 LEFT OUTER JOIN  dbo.returnedexpenses ON dbo.savedexpenses.expenseid = dbo.returnedexpenses.expenseid 
 ORDER BY primaryitem DESC, expenseid ASC
RETURN 0