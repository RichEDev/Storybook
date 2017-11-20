ALTER TABLE [dbo].[custom_entity_attribute_summary_columns]
    ADD CONSTRAINT [PK_custom_entity_attribute_summary_columns_columnid] PRIMARY KEY CLUSTERED ([columnid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

