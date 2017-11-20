CREATE TABLE [dbo].[ExpenseValidationThresholds]
(
   [ThresholdId] int NOT NULL CONSTRAINT PK_ExpenseValidationThresholds PRIMARY KEY IDENTITY(1,1),
   [Label] nvarchar(100) NOT NULL,
   [LowerBound] decimal NULL,
   [UpperBound] decimal NULL,
)
GO