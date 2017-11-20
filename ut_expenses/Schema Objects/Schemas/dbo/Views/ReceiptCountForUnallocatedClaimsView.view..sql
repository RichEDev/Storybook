CREATE VIEW [dbo].[ReceiptCountForUnallocatedClaimsView]
AS
SELECT dbo.savedexpenses.expenseid
	,CASE 
		WHEN dbo.savedexpenses.normalreceipt = 1
			THEN CASE 
					WHEN COUNT(dbo.savedexpenses_costcodes.savedcostcodeid) <= 1
						THEN 1
					ELSE CAST(1 AS DECIMAL(10, 4)) / CAST(COUNT(dbo.savedexpenses_costcodes.savedcostcodeid) AS DECIMAL(10, 4))
					END
		ELSE 0
		END AS numberofreceipts
	,CASE 
		WHEN dbo.savedexpenses.receiptattached = 1
			THEN CASE 
					WHEN COUNT(dbo.savedexpenses_costcodes.savedcostcodeid) <= 1
						THEN 1
					ELSE CAST(1 AS DECIMAL(10, 4)) / CAST(COUNT(dbo.savedexpenses_costcodes.savedcostcodeid) AS DECIMAL(10, 4))
					END
		ELSE 0
		END AS numberofattachedreceipts
FROM dbo.savedexpenses
LEFT OUTER JOIN dbo.savedexpenses_costcodes ON dbo.savedexpenses.expenseid = dbo.savedexpenses_costcodes.expenseid
GROUP BY dbo.savedexpenses.expenseid
	,dbo.savedexpenses.normalreceipt
	,dbo.savedexpenses.receiptattached