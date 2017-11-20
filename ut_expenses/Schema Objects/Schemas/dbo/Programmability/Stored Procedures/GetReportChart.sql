CREATE PROCEDURE [dbo].[GetReportChart]
	@reportId UNIQUEIDENTIFIER
AS
	SELECT 	reportId, chartType, chartTitle, showLegend, xaxis, yaxis, groupby, cumulative, charttitlefont, charttitlecolour, textfont, 
			textfontcolour, textbackgroundcolour, yaxisRange, legendPosition, showLabels, showValues, ShowPercent, enableHover, chartSize, CollectedThresholdPercent
	FROM ReportCharts WHERE reportId = @reportId
		
RETURN 0
