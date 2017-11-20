ALTER TABLE [dbo].[cars_attachments]
    ADD CONSTRAINT [FK_cars_attachments_cars] FOREIGN KEY ([id]) REFERENCES [dbo].[cars] ([carid]) ON DELETE CASCADE ON UPDATE NO ACTION;

