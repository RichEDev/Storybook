ALTER TABLE [dbo].[ReportCharts]
	ADD CONSTRAINT [FK_ReportCharts_ReportId]
	FOREIGN KEY (ReportId)
	REFERENCES [Reports] (ReportId)
