ALTER TABLE [dbo].[employee_readbroadcasts]
    ADD CONSTRAINT [FK_employee_readbroadcasts_broadcast_messages] FOREIGN KEY ([broadcastid]) REFERENCES [dbo].[broadcast_messages] ([broadcastid]) ON DELETE CASCADE ON UPDATE NO ACTION;

