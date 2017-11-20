ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_nonightsapp] DEFAULT (0) FOR [nonightsapp];

