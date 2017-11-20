ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [DF_reports_exporttype] DEFAULT ((3)) FOR [exporttype];

