ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_nopersonalguestsapp] DEFAULT (0) FOR [nopersonalguestsapp];

