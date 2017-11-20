ALTER TABLE [dbo].[messages]
    ADD CONSTRAINT [DF_messages_direction] DEFAULT (0) FOR [direction];

