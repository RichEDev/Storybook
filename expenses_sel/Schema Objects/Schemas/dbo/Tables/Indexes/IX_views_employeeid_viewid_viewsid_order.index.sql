CREATE NONCLUSTERED INDEX [IX_views_employeeid_viewid_viewsid_order]
    ON [dbo].[views]([employeeid] ASC, [viewid] ASC, [viewsid] ASC, [order] ASC)
    INCLUDE([fieldID]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

