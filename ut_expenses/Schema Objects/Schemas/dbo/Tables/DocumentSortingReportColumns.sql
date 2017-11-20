


CREATE TABLE [dbo].[DocumentSortingReportColumns](
	[DocumentSortingReportColumnId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentSortingReportId] [int] NOT NULL,
	[ColumnName] [nvarchar](255) NOT NULL,
	[SequenceOrder] [int] NOT NULL,
	[DocumentSortTypeId] [int] NOT NULL,
 CONSTRAINT [PK_DocumentReportSortingConfigurationColumns] PRIMARY KEY CLUSTERED 
(
	[DocumentSortingReportColumnId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) 
GO

ALTER TABLE [dbo].[DocumentSortingReportColumns]  WITH CHECK ADD  CONSTRAINT [FK_DocumentReportSortingConfigurationColumns_DocumentReports] FOREIGN KEY([DocumentSortingReportId])
REFERENCES [dbo].[DocumentSortingReports] ([DocumentSortingReportId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DocumentSortingReportColumns] CHECK CONSTRAINT [FK_DocumentReportSortingConfigurationColumns_DocumentReports]
GO

ALTER TABLE [dbo].[DocumentSortingReportColumns]  WITH CHECK ADD  CONSTRAINT [FK_DocumentReportSortingConfigurationColumns_DocumentSortType] FOREIGN KEY([DocumentSortTypeId])
REFERENCES [dbo].[DocumentSortType] ([DocumentSortTypeId])
GO

ALTER TABLE [dbo].[DocumentSortingReportColumns] CHECK CONSTRAINT [FK_DocumentReportSortingConfigurationColumns_DocumentSortType]
GO


