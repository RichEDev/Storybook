CREATE NONCLUSTERED INDEX [IX_custom_entity_attribute_summary_attributeid]
    ON [dbo].[custom_entity_attribute_summary]([attributeid] ASC, [order] ASC)
    INCLUDE([otm_attributeid]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

