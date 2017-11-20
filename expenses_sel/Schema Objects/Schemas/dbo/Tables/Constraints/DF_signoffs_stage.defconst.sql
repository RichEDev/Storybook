ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_stage] DEFAULT (0) FOR [stage];

