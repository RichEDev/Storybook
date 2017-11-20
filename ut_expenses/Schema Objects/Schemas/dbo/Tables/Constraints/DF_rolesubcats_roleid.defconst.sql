ALTER TABLE [dbo].[rolesubcats]
    ADD CONSTRAINT [DF_rolesubcats_roleid] DEFAULT (0) FOR [roleid];

