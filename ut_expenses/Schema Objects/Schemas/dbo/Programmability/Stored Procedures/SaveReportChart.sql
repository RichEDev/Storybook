CREATE PROCEDURE [dbo].[SaveReportChart]
	@ReportId UNIQUEIDENTIFIER  , 
	@ChartType TINYINT , 
	@ChartTitle NVARCHAR(100) , 
	@ShowLegend BIT , 
	@XAxis INT , 
	@YAxis INT , 
	@GroupBy INT ,
	@Cumulative BIT ,
	@ChartTitleFont INT, 
	@ChartTitleColour NVARCHAR(10) , 
	@TextFont INT, 
	@TextFontColour NVARCHAR(10) , 
	@TextBackgroundColour NVARCHAR(10) , 
	@YAxisRange TINYINT , 
	@LegendPosition TINYINT , 
	@ShowLabels BIT , 
	@ShowValues BIT , 
	@ShowPercent BIT , 
	@EnableHover BIT , 
	@ChartSize TINYINT ,
	@CollectedThresholdPercent INT
AS
	IF EXISTS (SELECT reportid FROM ReportCharts WHERE reportId = @reportId)
	BEGIN
		UPDATE ReportCharts SET chartType = @chartType, chartTitle = @chartTitle, showLegend = @showLegend, xaxis = @xaxis, yaxis = @yaxis,
		 groupby = @groupBy, cumulative = @cumulative, charttitlefont = @charttitlefont, charttitlecolour = @charttitlecolour, textfont = @textfont, 
			textfontcolour = @textfontcolour, textbackgroundcolour = @textbackgroundcolour, yaxisRange = @yaxisRange, legendPosition = @legendPosition, showLabels = @showLabels, showValues = @showValues, ShowPercent = @ShowPercent, enableHover = @enableHover, chartSize = @chartSize, CollectedThresholdPercent = @CollectedThresholdPercent
		WHERE reportId = @reportId
	END
	ELSE
	BEGIN
		INSERT INTO ReportCharts (reportId, chartType, chartTitle, showLegend, xaxis, yaxis, groupby, cumulative, charttitlefont, charttitlecolour, textfont, 
			textfontcolour, textbackgroundcolour, yaxisRange, legendPosition, showLabels, showValues, ShowPercent, enableHover, chartSize, CollectedThresholdPercent)
		VALUES (@reportId, @ChartType , @ChartTitle  , @ShowLegend  , @XAxis  , @YAxis  , @GroupBy  ,@Cumulative  ,@ChartTitleFont , 
		 @ChartTitleColour  ,@TextFont, @TextFontColour  , @TextBackgroundColour  , 
		 @YAxisRange , @LegendPosition  , @ShowLabels  , @ShowValues  , @ShowPercent  , @EnableHover  , 
			 @ChartSize, @CollectedThresholdPercent)
		
	END
RETURN 0
