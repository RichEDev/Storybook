ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [DF_reports_reportID] DEFAULT (newid()) FOR [reportID];

