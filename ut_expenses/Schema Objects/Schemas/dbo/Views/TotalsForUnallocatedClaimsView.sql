CREATE VIEW [dbo].[TotalsForUnallocatedClaimsView]
AS
SELECT        dbo.savedexpenses.expenseid, CASE WHEN COUNT(dbo.savedexpenses_costcodes.savedcostcodeid) 
                         = 0 THEN dbo.savedexpenses.total ELSE round(dbo.savedexpenses.total / COUNT(dbo.savedexpenses_costcodes.savedcostcodeid), 4) END AS splittotal
FROM            dbo.savedexpenses LEFT OUTER JOIN
                         dbo.savedexpenses_costcodes ON dbo.savedexpenses.expenseid = dbo.savedexpenses_costcodes.expenseid
GROUP BY dbo.savedexpenses.expenseid, dbo.savedexpenses.total
