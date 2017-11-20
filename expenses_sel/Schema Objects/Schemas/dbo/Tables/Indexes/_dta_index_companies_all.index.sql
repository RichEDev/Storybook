CREATE NONCLUSTERED INDEX [_dta_index_companies_all]
    ON [dbo].[companies]([archived] ASC, [showto] ASC, [company] ASC)
    INCLUDE([companyid], [postcode]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

