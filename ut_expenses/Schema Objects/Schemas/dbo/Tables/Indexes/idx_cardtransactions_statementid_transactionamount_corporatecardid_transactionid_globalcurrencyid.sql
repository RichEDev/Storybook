CREATE NONCLUSTERED INDEX [idx_cardtransactions_statementid_transactionamount_corporatecardid_transactionid_globalcurrencyid] ON [dbo].[card_transactions]
(
	[statementid] ASC,
	[transaction_amount] ASC,
	[corporatecardid] ASC,
	[transactionid] ASC,
	[globalcurrencyid] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]