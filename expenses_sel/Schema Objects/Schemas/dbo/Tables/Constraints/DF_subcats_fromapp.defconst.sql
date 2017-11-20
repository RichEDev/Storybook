ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_fromapp] DEFAULT ((0)) FOR [fromapp];

