CREATE VIEW dbo.reportcolumnsview
AS
SELECT     fieldid, groupby, sort, [order], aggfunction, funcsum, funcmax, funcmin, funcavg, funccount, isLiteral, literalname, literalvalue, length, format, removedecimals, 
                      pivottype, pivotorder, runtime, columntype, hidden, reportcolumnid, reportid, joinViaID, [system]
FROM         dbo.reportcolumns
UNION
SELECT     fieldID, groupby, sort, [order], aggfunction, funcsum, funcmax, funcmin, funcavg, funccount, isLiteral, literalname, literalvalue, length, format, removedecimals, 
                      pivottype, pivotorder, runtime, columntype, hidden, reportcolumnid, reportID, NULL as joinViaID, CAST(0 as BIT) as [system]
FROM         [$(targetMetabase)].dbo.reportcolumns AS reportcolumns_1


