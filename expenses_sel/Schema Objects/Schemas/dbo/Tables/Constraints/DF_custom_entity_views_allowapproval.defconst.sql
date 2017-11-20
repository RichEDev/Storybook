ALTER TABLE [dbo].[custom_entity_views]
    ADD CONSTRAINT [DF_custom_entity_views_allowapproval] DEFAULT ((0)) FOR [allowapproval];

