ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [DF_reports_forclaimants] DEFAULT (0) FOR [forclaimants];

