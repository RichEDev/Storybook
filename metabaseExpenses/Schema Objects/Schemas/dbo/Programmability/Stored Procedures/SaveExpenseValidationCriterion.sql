CREATE PROCEDURE [dbo].[SaveExpenseValidationCriterion]
 @id INT = 0,
 @fieldId UNIQUEIDENTIFIER = null,
 @accountId INT = null,
 @subcatId INT = null,
 @enabled bit = 1,
 @requirements NVARCHAR(4000),
 @fraudulentIfFailsVAT bit,
 @friendlyMessageFoundAndMatched NVARCHAR(200) = '',
 @friendlyMessageFoundNotMatched NVARCHAR(200) = '',
 @friendlyMessageFoundNotReadable NVARCHAR(200) = '',
 @friendlyMessageNotFound NVARCHAR(200) = ''
AS
BEGIN
 SET NOCOUNT ON;
 IF (@accountId IS NOT NULL AND (SELECT COUNT(accountid) FROM registeredusers WHERE accountid = @accountId) = 0) RETURN -2;
 IF (@fieldId IS NOT NULL AND (SELECT COUNT(fieldid) FROM fields_base WHERE fieldid = @fieldId) = 0) RETURN -3;
 IF (@id = 0)
  BEGIN
   INSERT INTO [dbo].[ExpenseValidationCriteria] ( FieldId, Requirements, AccountId, SubcatId, Enabled, FraudulentIfFailsVAT, FriendlyMessageFoundAndMatched, FriendlyMessageFoundNotMatched, FriendlyMessageFoundNotReadable, FriendlyMessageNotFound)
   VALUES ( @fieldId, @requirements, @accountId, @subcatId, @enabled, @fraudulentIfFailsVAT, @friendlyMessageFoundAndMatched, @friendlyMessageFoundNotMatched, @friendlyMessageFoundNotReadable, @friendlyMessageNotFound );
   RETURN SCOPE_IDENTITY();
  END
 ELSE
  BEGIN
   IF NOT EXISTS (SELECT CriterionId FROM [dbo].[ExpenseValidationCriteria] WHERE CriterionId = @id) RETURN -1;
   UPDATE [dbo].[ExpenseValidationCriteria]
   SET 
    FieldId = @fieldId,
    Requirements = @requirements,
    AccountId = @accountId,
    SubcatId = @subcatId,
    Enabled = @enabled,
    FraudulentIfFailsVAT = @fraudulentIfFailsVAT,
    FriendlyMessageFoundAndMatched = @friendlyMessageFoundAndMatched,
    FriendlyMessageFoundNotMatched = @friendlyMessageFoundNotMatched,
    FriendlyMessageFoundNotReadable = @friendlyMessageFoundNotReadable,
    FriendlyMessageNotFound = @friendlyMessageNotFound
   WHERE CriterionId = @id;
   RETURN @id;
  END
END