ALTER TABLE [dbo].[custom_entity_view_fields]
    ADD CONSTRAINT [FK_custom_entity_view_fields_custom_entity_views] FOREIGN KEY ([viewid]) REFERENCES [dbo].[custom_entity_views] ([viewid]) ON DELETE CASCADE ON UPDATE NO ACTION;

