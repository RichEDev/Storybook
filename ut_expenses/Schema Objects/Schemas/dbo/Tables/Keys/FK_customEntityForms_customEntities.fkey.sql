ALTER TABLE [dbo].[customEntityForms]
    ADD CONSTRAINT [FK_customEntityForms_customEntities] FOREIGN KEY ([entityid]) REFERENCES [dbo].[customEntities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

