ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_onholiday] DEFAULT (0) FOR [onholiday];

