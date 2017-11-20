ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_attendeesmand] DEFAULT (0) FOR [attendeesmand];

