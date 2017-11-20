ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_tipapp] DEFAULT (0) FOR [tipapp];

