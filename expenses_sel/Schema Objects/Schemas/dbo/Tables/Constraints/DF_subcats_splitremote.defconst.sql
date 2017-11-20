ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_splitremote] DEFAULT (0) FOR [splitremote];

