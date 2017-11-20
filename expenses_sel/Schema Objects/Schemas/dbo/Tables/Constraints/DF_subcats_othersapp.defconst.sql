ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_othersapp] DEFAULT (0) FOR [othersapp];

