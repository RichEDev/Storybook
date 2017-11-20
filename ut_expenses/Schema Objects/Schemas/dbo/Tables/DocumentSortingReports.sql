
CREATE TABLE [dbo].[DocumentSortingReports](
	[DocumentSortingReportId] [int] IDENTITY(1,1) NOT NULL,
	[ReportSortingConfigurationId] [int] NOT NULL,
	[ReportName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_DocumentReports] PRIMARY KEY CLUSTERED 
(
	[DocumentSortingReportId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) 

GO

ALTER TABLE [dbo].[DocumentSortingReports]  WITH CHECK ADD  CONSTRAINT [FK_DocumentReports_DocumentReportSortingConfigurations] FOREIGN KEY([ReportSortingConfigurationId])
REFERENCES [dbo].[DocumentReportSortingConfigurations] ([ReportSortingConfigurationId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DocumentSortingReports] CHECK CONSTRAINT [FK_DocumentReports_DocumentReportSortingConfigurations]
GO