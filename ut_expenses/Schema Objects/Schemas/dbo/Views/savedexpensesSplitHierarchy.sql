CREATE VIEW [dbo].[savedexpensesSplitHierarchy]
AS
WITH cte AS (SELECT     savedexpenses_splititems.primaryitem, splititem
                             FROM         dbo.savedexpenses_splititems 
                             UNION ALL
                             SELECT     cte_2.primaryitem, t.splititem
                             FROM         dbo.savedexpenses_splititems AS t INNER JOIN
                                                   cte AS cte_2 ON t.primaryitem = cte_2.splititem)
    SELECT     TOP (100) PERCENT primaryitem, splititem
     FROM         cte AS cte_1
     ORDER BY primaryitem