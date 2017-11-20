ALTER TABLE [dbo].[custom_entity_attribute_summary]
    ADD CONSTRAINT [PK_custom_entity_attribute_summary_summaryid] PRIMARY KEY CLUSTERED ([summary_attributeid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

