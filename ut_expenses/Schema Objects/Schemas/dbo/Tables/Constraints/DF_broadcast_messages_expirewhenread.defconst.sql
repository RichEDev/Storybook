ALTER TABLE [dbo].[broadcast_messages]
    ADD CONSTRAINT [DF_broadcast_messages_expirewhenread] DEFAULT ((0)) FOR [expirewhenread];

