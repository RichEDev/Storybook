CREATE NONCLUSTERED INDEX [_dta_index_ExpenseValidationResults_387_1221592995__K2_K3_1_4_5_6_7_8_9_10] ON [dbo].[ExpenseValidationResults] 
(
	[ExpenseId] ASC,
	[CriterionId] ASC
)
	INCLUDE ( [ResultId],
	[ValidationStatusBusiness],
	[ValidationStatusVAT],
	[PossiblyFraudulent],
	[ValidationCompleted],
	[Comments],
	[Data],
	[MatchingResult]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]