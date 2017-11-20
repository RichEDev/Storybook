ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_sendmail] DEFAULT (0) FOR [sendmail];

