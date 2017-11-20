ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_attendeesapp] DEFAULT (0) FOR [attendeesapp];

