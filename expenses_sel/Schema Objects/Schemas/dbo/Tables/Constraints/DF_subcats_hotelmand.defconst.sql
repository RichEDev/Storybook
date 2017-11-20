ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_hotelmand] DEFAULT (0) FOR [hotelmand];

