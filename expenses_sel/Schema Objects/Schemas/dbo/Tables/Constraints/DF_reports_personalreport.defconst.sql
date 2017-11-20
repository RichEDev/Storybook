ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [DF_reports_personalreport] DEFAULT (0) FOR [personalreport];

