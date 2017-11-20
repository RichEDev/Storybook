ALTER TABLE [dbo].[rolesubcats]
    ADD CONSTRAINT [DF_rolesubcats_maximum] DEFAULT (0) FOR [maximum];

