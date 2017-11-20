CREATE TABLE [dbo].[ExpenseValidationReasonResultMapping]
(
   ThresholdId int NOT NULL CONSTRAINT FK_ExpenseValidationReasonResultMapping_ExpenseValidationThresholds FOREIGN KEY REFERENCES ExpenseValidationThresholds (ThresholdId),
   CriterionId int NOT NULL CONSTRAINT FK_ExpenseValidationReasonResultMapping_ExpenseValidationCriteria FOREIGN KEY REFERENCES ExpenseValidationCriteria (CriterionId),
   ReasonId int NOT NULL,
   ResultStatus int NOT NULL,
   Primary Key (ThresholdId, CriterionId, ReasonId)
)
GO