ALTER TABLE [dbo].[custom_entity_view_filters]
    ADD CONSTRAINT [FK_custom_entity_view_filters_custom_entity_views] FOREIGN KEY ([viewid]) REFERENCES [dbo].[custom_entity_views] ([viewid]) ON DELETE CASCADE ON UPDATE NO ACTION;

