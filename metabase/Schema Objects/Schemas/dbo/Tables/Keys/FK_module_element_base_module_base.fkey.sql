ALTER TABLE [dbo].[moduleElementBase]
    ADD CONSTRAINT [FK_module_element_base_module_base] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[moduleBase] ([moduleID]) ON DELETE NO ACTION ON UPDATE NO ACTION NOT FOR REPLICATION;

