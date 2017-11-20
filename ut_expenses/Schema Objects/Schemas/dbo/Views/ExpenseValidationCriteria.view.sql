CREATE VIEW ExpenseValidationCriteria 
AS
SELECT     CriterionId, Requirements, FieldId, AccountId, SubcatId, Enabled, FraudulentIfFailsVAT, FriendlyMessageFoundAndMatched, FriendlyMessageFoundNotMatched, FriendlyMessageFoundNotReadable, FriendlyMessageNotFound
FROM       [$(targetMetabase)].[dbo].[ExpenseValidationCriteria]