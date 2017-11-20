CREATE NONCLUSTERED INDEX [IX_custom_entity_attribute_summary_columns_attributeid]
    ON [dbo].[custom_entity_attribute_summary_columns]([attributeid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

