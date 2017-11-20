
CREATE VIEW [dbo].[reportcriteriaview]
AS
SELECT     fieldid, condition, value1, value2, andor, [order], runtime, groupnumber, reportID, criteriaid
FROM         dbo.reportcriteria
UNION
SELECT     fieldid, condition, value1, value2, andor, [order], runtime, groupnumber, reportid, criteriaid
FROM         [$(metabaseExpenses)].dbo.reportcriteria AS reportcriteria_1

