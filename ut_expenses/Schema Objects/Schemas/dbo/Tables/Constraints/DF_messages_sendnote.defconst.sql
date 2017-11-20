ALTER TABLE [dbo].[messages]
    ADD CONSTRAINT [DF_messages_sendnote] DEFAULT (0) FOR [sendnote];

