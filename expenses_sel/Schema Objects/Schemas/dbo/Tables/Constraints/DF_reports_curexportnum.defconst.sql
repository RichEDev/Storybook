ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [DF_reports_curexportnum] DEFAULT (1) FOR [curexportnum];

