ALTER TABLE [dbo].[rolesubcats]
    ADD CONSTRAINT [DF_rolesubcats_subcatid] DEFAULT (0) FOR [subcatid];

