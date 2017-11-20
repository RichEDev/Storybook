ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [FK_custom_entity_attributes_workflows] FOREIGN KEY ([workflowid]) REFERENCES [dbo].[workflows] ([workflowID]) ON DELETE SET NULL ON UPDATE NO ACTION;

