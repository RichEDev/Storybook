CREATE NONCLUSTERED INDEX [IX_savedexpenses_current_claimid_tempallow_itemtype_normalreceipt_expenseid_subcatid]
    ON [dbo].[savedexpenses_current]([claimid] ASC, [tempallow] ASC, [itemtype] ASC, [normalreceipt] ASC, [expenseid] ASC, [subcatid] ASC)
    INCLUDE([total]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

