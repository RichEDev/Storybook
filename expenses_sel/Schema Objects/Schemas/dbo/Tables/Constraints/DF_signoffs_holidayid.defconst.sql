ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_holidayid] DEFAULT (0) FOR [holidayid];

