ALTER TABLE [dbo].[savings]
    ADD CONSTRAINT [DF_Savings_Amount] DEFAULT ((0)) FOR [Amount];

