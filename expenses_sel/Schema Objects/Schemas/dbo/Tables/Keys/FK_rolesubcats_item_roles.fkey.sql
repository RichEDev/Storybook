ALTER TABLE [dbo].[rolesubcats]
    ADD CONSTRAINT [FK_rolesubcats_item_roles] FOREIGN KEY ([roleid]) REFERENCES [dbo].[item_roles] ([itemroleid]) ON DELETE CASCADE ON UPDATE NO ACTION;

