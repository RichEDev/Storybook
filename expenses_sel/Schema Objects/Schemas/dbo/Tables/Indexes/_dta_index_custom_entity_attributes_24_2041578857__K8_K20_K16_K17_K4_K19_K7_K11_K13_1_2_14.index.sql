CREATE NONCLUSTERED INDEX [_dta_index_custom_entity_attributes_24_2041578857__K8_K20_K16_K17_K4_K19_K7_K11_K13_1_2_14]
    ON [dbo].[custom_entity_attributes]([fieldtype] ASC, [relationshiptype] ASC, [fieldid] ASC, [relatedtable] ASC, [display_name] ASC, [is_key_field] ASC, [mandatory] ASC, [modifiedon] ASC, [maxlength] ASC)
    INCLUDE([attributeid], [entityid], [format]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

