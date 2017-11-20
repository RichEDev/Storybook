CREATE PROCEDURE [dbo].[GetReportSourcesSummary]
	@mergeProjectId int
AS
	select mergesourceid, document_mergesources.reportid, reportname from dbo.document_mergesources 
		inner join reports on reports.reportid = document_mergesources.reportid 
		left join ReportCharts on reports.reportID = ReportCharts.ReportId 
		where mergeprojectid = @mergeprojectid and ReportCharts.ReportId is null;
RETURN 0
