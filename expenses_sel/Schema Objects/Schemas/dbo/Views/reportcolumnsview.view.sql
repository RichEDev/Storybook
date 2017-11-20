

CREATE VIEW [dbo].[reportcolumnsview]
AS
SELECT     fieldid, groupby, sort, [order], aggfunction, funcsum, funcmax, funcmin, funcavg, funccount, isLiteral, literalname, 
                      literalvalue, length, format, removedecimals, pivottype, pivotorder, runtime, columntype, hidden, reportcolumnid, reportid
FROM         dbo.reportcolumns
UNION
SELECT     fieldID, groupby, sort, [order], aggfunction, funcsum, funcmax, funcmin, funcavg, funccount, isLiteral, literalname, 
                      literalvalue, length, format, removedecimals, pivottype, pivotorder, runtime, columntype, hidden, reportcolumnid, reportID
FROM         [$(metabaseExpenses)].dbo.reportcolumns AS reportcolumns_1


