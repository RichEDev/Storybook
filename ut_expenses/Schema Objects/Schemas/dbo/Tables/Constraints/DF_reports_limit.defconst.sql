ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [DF_reports_limit] DEFAULT ((0)) FOR [limit];

