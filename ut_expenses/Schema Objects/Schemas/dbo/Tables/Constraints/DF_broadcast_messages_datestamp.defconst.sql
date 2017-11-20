ALTER TABLE [dbo].[broadcast_messages]
    ADD CONSTRAINT [DF_broadcast_messages_datestamp] DEFAULT (getdate()) FOR [datestamp];

