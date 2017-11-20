CREATE NONCLUSTERED INDEX [_dta_index_card_transactions_24_5835333__K1_K5_K14_K11_2_4_6_7_8_9_12]
    ON [dbo].[card_transactions]([transactionid] ASC, [corporatecardid] ASC, [globalcountryid] ASC, [globalcurrencyid] ASC)
    INCLUDE([card_number], [description], [exchangerate], [original_amount], [statementid], [transaction_amount], [transaction_date]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

