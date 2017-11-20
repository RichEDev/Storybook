ALTER TABLE [dbo].[flagAssociatedFields]
    ADD CONSTRAINT [FK_flagAssociatedFields_flags] FOREIGN KEY ([flagID]) REFERENCES [dbo].[flags] ([flagID]) ON DELETE CASCADE ON UPDATE NO ACTION;

