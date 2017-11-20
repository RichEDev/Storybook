ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [DF_reports_readonly] DEFAULT (0) FOR [readonly];

