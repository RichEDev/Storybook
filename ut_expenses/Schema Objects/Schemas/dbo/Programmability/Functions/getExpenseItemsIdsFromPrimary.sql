CREATE FUNCTION [dbo].[getExpenseItemIdsFromPrimary] 
(
 @expenseid int
)
RETURNS table 
AS
RETURN
(
 -- Declare the return variable here
 
 WITH cte AS
 (
 
    SELECT 
       
       splititem
    FROM savedexpenses_splititems
    WHERE primaryitem =@expenseid

    UNION ALL

    SELECT 
       
       t.splititem
    FROM savedexpenses_splititems t
    INNER JOIN cte ON t.primaryitem = cte.splititem
)
SELECT splititem FROM cte
)