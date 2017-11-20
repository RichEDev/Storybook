ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_categoryid] DEFAULT (0) FOR [categoryid];

