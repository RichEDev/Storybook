ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_receiptattached] DEFAULT ((0)) FOR [receiptattached];

