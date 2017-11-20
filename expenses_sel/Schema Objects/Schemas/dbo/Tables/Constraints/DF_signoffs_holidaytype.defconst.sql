ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_holidaytype] DEFAULT (0) FOR [holidaytype];

