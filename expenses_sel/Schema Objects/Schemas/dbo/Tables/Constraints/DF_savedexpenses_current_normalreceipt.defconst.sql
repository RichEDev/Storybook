ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_normalreceipt] DEFAULT ((0)) FOR [normalreceipt];

