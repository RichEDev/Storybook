ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_receiptattached] DEFAULT (0) FOR [receiptattached];

