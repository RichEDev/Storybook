ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_noremoteworkersapp] DEFAULT (0) FOR [noremoteworkersapp];

