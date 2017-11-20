ALTER TABLE [dbo].[custom_entity_attribute_list_items]
    ADD CONSTRAINT [PK_custom_entity_attribute_list_items] PRIMARY KEY CLUSTERED ([attributeid] ASC, [item] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

