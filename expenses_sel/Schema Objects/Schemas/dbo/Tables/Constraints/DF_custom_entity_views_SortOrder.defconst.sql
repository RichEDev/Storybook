ALTER TABLE [dbo].[custom_entity_views]
    ADD CONSTRAINT [DF_custom_entity_views_SortOrder] DEFAULT ((0)) FOR [SortOrder];

