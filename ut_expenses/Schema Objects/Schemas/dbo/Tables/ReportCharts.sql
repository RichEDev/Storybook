CREATE TABLE [dbo].[ReportCharts]
(
    [ReportId] UNIQUEIDENTIFIER NOT NULL , 
    [ChartType] TINYINT NOT NULL, 
    [ChartTitle] NVARCHAR(100) NULL, 
    [ShowLegend] BIT NULL, 
    [XAxis] INT NULL, 
    [YAxis] INT NULL, 
    [GroupBy] INT NULL,
    [Cumulative] BIT NULL,
    [ChartTitleFont] INT NOT NULL DEFAULT 12, 
    [ChartTitleColour] NVARCHAR(10) NULL, 
    [TextFont] INT NOT NULL DEFAULT 10, 
    [TextFontColour] NVARCHAR(10) NULL, 
    [TextBackgroundColour] NVARCHAR(10) NULL, 
    [YAxisRange] TINYINT NULL, 
    [LegendPosition] TINYINT NULL, 
    [ShowLabels] BIT NULL, 
    [ShowValues] BIT NULL, 
    [ShowPercent] BIT NULL, 
    [EnableHover] BIT NULL, 
    [ChartSize] TINYINT NULL, 
    [CollectedThresholdPercent] INT NULL
    
)
