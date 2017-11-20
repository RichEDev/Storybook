ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_nodirectorsapp] DEFAULT (0) FOR [nodirectorsapp];

