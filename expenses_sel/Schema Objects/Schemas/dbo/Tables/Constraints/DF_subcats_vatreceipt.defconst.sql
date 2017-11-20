ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_vatreceipt] DEFAULT (0) FOR [vatreceipt];

