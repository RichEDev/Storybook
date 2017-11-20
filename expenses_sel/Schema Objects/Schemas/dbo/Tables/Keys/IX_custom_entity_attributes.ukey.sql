ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [IX_custom_entity_attributes] UNIQUE NONCLUSTERED ([entityid] ASC, [attribute_name] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY];

