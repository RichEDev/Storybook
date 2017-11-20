ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_signofftype] DEFAULT (0) FOR [signofftype];

