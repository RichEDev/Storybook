CREATE VIEW [dbo].[userdefinedInformation]
AS
SELECT     TOP (100) PERCENT tableid AS parentTableID, description AS appliesTo, userdefined_table,
                          (SELECT     COUNT(*) AS Expr1
                            FROM          dbo.userdefinedGroupings
                            WHERE      (associatedTable = dbo.tables.tableid)) AS groupCount,
                          (SELECT     COUNT(*) AS Expr1
                            FROM          dbo.userdefined INNER JOIN
                                                   dbo.tables AS udfTable2 ON dbo.userdefined.tableid = udfTable2.tableid
                            WHERE      (udfTable2.tableid = dbo.tables.userdefined_table)) AS fieldCount
FROM         dbo.tables
WHERE     (hasUserDefinedFields = 1) AND (parentTableID IS NULL) AND (userdefined_table IS NOT NULL)
ORDER BY appliesTo

