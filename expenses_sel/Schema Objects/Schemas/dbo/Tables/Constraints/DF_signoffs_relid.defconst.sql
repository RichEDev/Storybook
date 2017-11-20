ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_relid] DEFAULT (0) FOR [relid];

