CREATE TABLE [dbo].[ExpenseValidationResults]
(
	[ResultId]					int IDENTITY(1,1)	NOT NULL	CONSTRAINT [PK_ExpenseValidationResults] PRIMARY KEY CLUSTERED,
	[ExpenseId]					int					NOT NULL	CONSTRAINT [FK_ExpenseValidationResults_SavedExpenses] FOREIGN KEY REFERENCES [dbo].[savedexpenses] (expenseid),
	[CriterionId]				int					NOT NULL,	
	[ValidationStatusBusiness]	int					NOT NULL default(0),
	[ValidationStatusVAT]		int					NOT NULL default(0),
	[PossiblyFraudulent]		bit					NOT NULL default(0),
	[ValidationCompleted]		datetime			NULL,
	[Comments]					nvarchar(4000)		NULL,
	[Data]						XML					NULL,
	[MatchingResult]			int					NOT NULL default(1),
)

