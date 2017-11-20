CREATE FUNCTION [dbo].[getExpenseItemIdsFromPrimary] (@expenseid INT)
RETURNS TABLE
AS
RETURN (
		WITH cte AS (
				SELECT @expenseid AS splititem
				
				UNION ALL
				
				SELECT splititem
				FROM savedexpenses_splititems
				WHERE primaryitem = @expenseid
				
				UNION ALL
				
				SELECT t.splititem
				FROM savedexpenses_splititems t
				INNER JOIN cte ON t.primaryitem = cte.splititem
				)
		SELECT splititem
		FROM cte
		)