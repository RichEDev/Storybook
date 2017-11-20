ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_notify] DEFAULT (0) FOR [notify];

