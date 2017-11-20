ALTER TABLE [dbo].[flagAssociatedRoles]
    ADD CONSTRAINT [FK_flagAssociatedRoles_item_roles] FOREIGN KEY ([roleID]) REFERENCES [dbo].[item_roles] ([itemroleid]) ON DELETE CASCADE ON UPDATE NO ACTION;

