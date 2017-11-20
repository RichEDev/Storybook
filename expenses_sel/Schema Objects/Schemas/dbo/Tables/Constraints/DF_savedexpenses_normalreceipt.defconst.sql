ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_normalreceipt] DEFAULT (0) FOR [normalreceipt];

