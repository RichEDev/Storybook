ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_nopassengersapp] DEFAULT (0) FOR [nopassengersapp];

