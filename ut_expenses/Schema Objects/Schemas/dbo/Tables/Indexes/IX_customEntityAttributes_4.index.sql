CREATE NONCLUSTERED INDEX [IX_customEntityAttributes_4]
    ON [dbo].[customEntityAttributes]([entityid] ASC, [relationshiptype] ASC, [fieldtype] ASC, [fieldid] ASC, [display_name] ASC, [is_key_field] ASC, [mandatory] ASC, [modifiedon] ASC, [maxlength] ASC)
    INCLUDE([attributeid], [format]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

