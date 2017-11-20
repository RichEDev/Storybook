CREATE VIEW dbo.reportcriteriaview
AS
SELECT     fieldid, condition, value1, value2, andor, [order], runtime, groupnumber, reportID, criteriaid, joinViaID
FROM         dbo.reportcriteria
UNION
SELECT     fieldid, condition, value1, value2, andor, [order], runtime, groupnumber, reportid, criteriaid, 0 as joinViaID
FROM         [$(targetMetabase)].dbo.reportcriteria AS reportcriteria_1

