CREATE NONCLUSTERED INDEX [IX_savedexpenses_claimid_tempallow_normalreceipt_expenseid_itemtype_subcatid]
    ON [dbo].[savedexpenses]([claimid] ASC, [tempallow] ASC, [normalreceipt] ASC, [expenseid] ASC, [itemtype] ASC, [subcatid] ASC)
    INCLUDE([amountpayable], [total]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

