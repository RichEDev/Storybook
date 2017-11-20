ALTER TABLE [dbo].[userdefined_list_items]
    ADD CONSTRAINT [FK_userdefined_list_items_userdefined] FOREIGN KEY ([userdefineid]) REFERENCES [dbo].[userdefined] ([userdefineid]) ON DELETE CASCADE ON UPDATE NO ACTION;

