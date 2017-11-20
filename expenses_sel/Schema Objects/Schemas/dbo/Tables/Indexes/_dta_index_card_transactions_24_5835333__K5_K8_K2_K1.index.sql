CREATE NONCLUSTERED INDEX [_dta_index_card_transactions_24_5835333__K5_K8_K2_K1]
    ON [dbo].[card_transactions]([corporatecardid] ASC, [transaction_amount] ASC, [statementid] ASC, [transactionid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

