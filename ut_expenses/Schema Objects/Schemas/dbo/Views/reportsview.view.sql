CREATE VIEW [dbo].[reportsview]
AS
WITH reportColumnCount (ReportId, ColumnCount, summary) AS (
SELECT reportid, count(*), CASE WHEN MAX(CAST(funcsum AS int)) = 1 or MAX(CAST(funcmax AS int)) = 1 or MAX(CAST(funcmin AS int)) = 1 or MAX(CAST(funcavg AS int)) = 1 or MAX(CAST(funccount AS int)) = 1 THEN 2 ELSE 1 END AS summary from reportcolumns GROUP BY ReportID
)

SELECT     reports.reportID, subAccountId, 0 as globalReport, reports.employeeid, reportname, description, personalreport, curexportnum, lastexportdate, footerreport, reports.folderid, readonly, forclaimants, allowexport, 
                      exporttype, reports.CreatedOn, reports.CreatedBy, reports.ModifiedOn, reports.ModifiedBy, limit, basetable, reports.module AS reportArea, NULL AS staticReportSQL, foldername as category, employees.surname + ', ' + employees.title + ' ' + employees.firstname as owner, reportfolders.personal as personalFolder, ISNULL(summary, 0) as reportType, useJoinVia, showChart
                      ,ColumnCount
FROM         dbo.reports
	left join reportfolders on reportfolders.folderid = reports.folderid
	left join employees on employees.employeeid = reports.employeeid
	left join reportColumnCount on reportColumnCount.ReportId = reports.reportID
UNION ALL
SELECT     reports.reportid, NULL AS subAccountId, 1 as globalReport, NULL AS employeeid, reportname, description, 0 AS personalreport, curexportnum, lastexportdate, footerreport, reports.folderid, readonly, 
                      forclaimants, allowexport, exporttype, reports.CreatedOn, reports.CreatedBy, reports.ModifiedOn, reports.ModifiedBy, limit, basetable, module AS reportArea, staticReportSQL, report_folders.foldername, 'Standard Owner' as owner, 0 as personalFolder, [$(targetMetabase)].dbo.getReportType(reports.reportid) as reportType, CAST(0 as bit) as useJoinVia, CAST(0 as tinyInt) as showChart
                      , 1 as Summary
FROM         [$(targetMetabase)].dbo.reports
	left join [$(targetMetabase)].dbo.report_folders on [$(targetMetabase)].dbo.report_folders.folderid = reports.folderid
