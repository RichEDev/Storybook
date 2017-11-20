ALTER TABLE [dbo].[moduleElementBase]
    ADD CONSTRAINT [FK_module_element_base_elements_base] FOREIGN KEY ([elementID]) REFERENCES [dbo].[elementsBase] ([elementID]) ON DELETE NO ACTION ON UPDATE NO ACTION NOT FOR REPLICATION;

