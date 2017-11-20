CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_previous_47_90535456__K47_K18_K11_K1]
    ON [dbo].[savedexpenses_previous]([carid] ASC, [claimid] ASC, [date] ASC, [expenseid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

