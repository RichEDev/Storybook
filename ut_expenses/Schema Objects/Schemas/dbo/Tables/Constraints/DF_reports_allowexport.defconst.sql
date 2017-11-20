ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [DF_reports_allowexport] DEFAULT (0) FOR [allowexport];

