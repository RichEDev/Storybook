CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_current_20_666537508__K18_K1_K7_K8_K9_K10]
    ON [dbo].[savedexpenses_current]([claimid] ASC, [expenseid] ASC, [net] ASC, [vat] ASC, [total] ASC, [subcatid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

