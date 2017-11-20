ALTER TABLE [dbo].[custom_entities]
    ADD CONSTRAINT [DF_custom_entities_systemview] DEFAULT ((0)) FOR [systemview];

