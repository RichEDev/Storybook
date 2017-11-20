ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_vatapp] DEFAULT (0) FOR [vatapp];

