CREATE VIEW dbo.reportfolders
AS
SELECT     folderID, 2 AS reportArea, foldername, employeeid, personal
FROM         dbo.report_folders
UNION
SELECT     folderid, 1 AS reportArea, foldername, NULL AS Expr1, CAST(0 AS BIT) AS personal
FROM         [$(targetMetabase)].dbo.report_folders AS report_folders_1

