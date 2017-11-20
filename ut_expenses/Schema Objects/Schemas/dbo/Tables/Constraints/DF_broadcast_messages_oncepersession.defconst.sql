ALTER TABLE [dbo].[broadcast_messages]
    ADD CONSTRAINT [DF_broadcast_messages_oncepersession] DEFAULT ((0)) FOR [oncepersession];

