ALTER TABLE [dbo].[rolesubcats]
    ADD CONSTRAINT [DF_rolesubcats_receiptmaximum] DEFAULT (0) FOR [receiptmaximum];

