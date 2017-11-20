CREATE PROCEDURE [dbo].[GetExpenseItems]
	@claimid int,
	@expenseid int
AS
	
SELECT savedexpenses.*,dbo.float_allocations.floatid, dbo.returnedexpenses.note,  dbo.returnedexpenses.dispute, dbo.returnedexpenses.corrected 
	FROM  dbo.savedexpenses LEFT OUTER JOIN 
                           dbo.float_allocations ON dbo.savedexpenses.expenseid = dbo.float_allocations.expenseid LEFT OUTER JOIN 
                           dbo.returnedexpenses ON dbo.savedexpenses.expenseid = dbo.returnedexpenses.expenseid 
                           WHERE               (@claimid is null or claimid = @claimid)  and 
                            (@expenseid is null or savedexpenses.expenseid in (SELECT splititem FROM dbo.getExpenseItemIdsFromPrimary(@expenseid)))
							ORDER BY primaryitem DESC, expenseid ASC
RETURN 0
