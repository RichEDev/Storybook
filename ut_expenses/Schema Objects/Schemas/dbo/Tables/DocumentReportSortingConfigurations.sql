CREATE TABLE [dbo].[DocumentReportSortingConfigurations](
	[ReportSortingConfigurationId] [int] IDENTITY(1,1) NOT NULL,
	[GroupingId] [int] NOT NULL,
	[MergeProjectId] [int] NOT NULL,
 CONSTRAINT [PK_DocumentReportSortingColumns] PRIMARY KEY CLUSTERED 
(
	[ReportSortingConfigurationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY])

GO

ALTER TABLE [dbo].[DocumentReportSortingConfigurations]  WITH CHECK ADD  CONSTRAINT [FK_DocumentReportSortingConfigurations_DocumentGroupingConfigurations] FOREIGN KEY([GroupingId], [MergeProjectId])
REFERENCES [dbo].[DocumentGroupingConfigurations] ([GroupingId], [MergeProjectId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DocumentReportSortingConfigurations] CHECK CONSTRAINT [FK_DocumentReportSortingConfigurations_DocumentGroupingConfigurations]
