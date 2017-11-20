CREATE NONCLUSTERED INDEX [_dta_index_savedexpensesFlags_15_525728097__K2_K1_K12_K11_3_4_5_6_7_8_9_10] ON [dbo].[savedexpensesFlags] 
(
	[expenseid] ASC,
	[flaggedItemId] ASC,
	[claimantJustificationDelegateID] ASC,
	[stepNumber] ASC
)
INCLUDE ( [flagID],
[flagType],
[flagDescription],
[flagText],
[duplicateExpenseID],
[flagColour],
[claimantJustification],
[exceededLimit]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]