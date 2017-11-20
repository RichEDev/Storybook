ALTER TABLE [dbo].[menu_structure_base]
    ADD CONSTRAINT [FK_menu_structure_menu_structure] FOREIGN KEY ([parentid]) REFERENCES [dbo].[menu_structure_base] ([menuid]) ON DELETE NO ACTION ON UPDATE NO ACTION NOT FOR REPLICATION;

