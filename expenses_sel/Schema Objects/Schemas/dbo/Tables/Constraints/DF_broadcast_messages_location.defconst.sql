ALTER TABLE [dbo].[broadcast_messages]
    ADD CONSTRAINT [DF_broadcast_messages_location] DEFAULT ((1)) FOR [location];

