CREATE NONCLUSTERED INDEX [_dta_index_savedExpensesFlagsApproverJustif_8_2062254119__K2_K3_4_5_6_7] ON [dbo].[savedExpensesFlagsApproverJustifications] 
(
	[flaggedItemId] ASC,
	[stage] ASC
)
INCLUDE ( [approverId],
[justification],
[datestamp],
[delegateID]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
