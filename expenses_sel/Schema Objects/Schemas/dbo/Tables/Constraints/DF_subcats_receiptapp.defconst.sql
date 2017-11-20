ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_receiptapp] DEFAULT (0) FOR [receiptapp];

