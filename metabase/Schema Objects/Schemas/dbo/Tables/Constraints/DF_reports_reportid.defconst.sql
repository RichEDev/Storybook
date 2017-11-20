ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [DF_reports_reportid] DEFAULT (newid()) FOR [reportid];

