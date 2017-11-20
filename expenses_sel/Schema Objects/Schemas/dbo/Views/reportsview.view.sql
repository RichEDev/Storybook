
CREATE VIEW [dbo].[reportsview]
AS
SELECT     reportID, subAccountId, employeeid, reportname, description, personalreport, curexportnum, lastexportdate, footerreport, folderid, readonly, forclaimants, allowexport, 
                      exporttype, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, limit, basetable, 2 AS reportArea, NULL AS staticReportSQL
FROM         dbo.reports
UNION
SELECT     reportid, NULL as subAccountId ,NULL AS employeeid, reportname, description, 0 AS personalreport, curexportnum, lastexportdate, footerreport, folderid, readonly, 
                      forclaimants, allowexport, exporttype, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, limit, basetable, 1 AS reportArea, staticReportSQL
FROM         [$(metabaseExpenses)].dbo.reports AS reports_1


