ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_singlesignoff] DEFAULT (0) FOR [singlesignoff];

