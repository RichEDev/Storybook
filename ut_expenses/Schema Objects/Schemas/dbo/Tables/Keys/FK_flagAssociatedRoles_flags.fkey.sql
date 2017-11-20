ALTER TABLE [dbo].[flagAssociatedRoles]
    ADD CONSTRAINT [FK_flagAssociatedRoles_flags] FOREIGN KEY ([flagID]) REFERENCES [dbo].[flags] ([flagID]) ON DELETE CASCADE ON UPDATE NO ACTION;

