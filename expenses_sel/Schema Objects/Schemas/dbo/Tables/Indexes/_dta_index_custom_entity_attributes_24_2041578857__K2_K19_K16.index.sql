CREATE NONCLUSTERED INDEX [_dta_index_custom_entity_attributes_24_2041578857__K2_K19_K16]
    ON [dbo].[custom_entity_attributes]([entityid] ASC, [is_key_field] ASC, [fieldid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

