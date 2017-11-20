ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_amount] DEFAULT (0) FOR [amount];

