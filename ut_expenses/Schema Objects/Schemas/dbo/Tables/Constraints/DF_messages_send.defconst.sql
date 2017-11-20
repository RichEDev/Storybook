ALTER TABLE [dbo].[messages]
    ADD CONSTRAINT [DF_messages_send] DEFAULT (1) FOR [send];

