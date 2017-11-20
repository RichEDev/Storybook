ALTER TABLE [dbo].[rolesubcats]
    ADD CONSTRAINT [DF_rolesubcats_preapproval] DEFAULT (0) FOR [preapproval];

