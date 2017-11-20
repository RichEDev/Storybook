CREATE PROCEDURE [dbo].[ToggleKnowledgeCustomArticlePublished]
 @KnowledgeCustomArticleId INT,
 @UserID INT, 
 @DelegateID INT
AS

DECLARE @count INT;

IF (@KnowledgeCustomArticleId > 0)
BEGIN
 -- Get and flip the current archived status
 DECLARE @Published BIT
 DECLARE @NewPublished BIT
 SELECT @Published = [Published] FROM [KnowledgeCustomArticles] WHERE [KnowledgeCustomArticleId] = @KnowledgeCustomArticleId;
 SELECT @NewPublished = @Published ^ 1;
 
 DECLARE @RecordTitle NVARCHAR(255);
 SET @RecordTitle = (SELECT [Title] FROM [KnowledgeCustomArticles] WHERE [KnowledgeCustomArticleId] = @KnowledgeCustomArticleId);
 
 -- Perform the update
 UPDATE [KnowledgeCustomArticles]
 SET    [Published] = @NewPublished,
     [ModifiedOn] = GETUTCDATE(),
     [ModifiedBy] = CASE WHEN @DelegateID IS NULL THEN @UserID ELSE @DelegateID END
 WHERE  [KnowledgeCustomArticleId] = @KnowledgeCustomArticleId;
 
 -- Update the audit log
 EXEC addUpdateEntryToAuditLog @userid, @delegateID, 31, @KnowledgeCustomArticleId, '97326562-E049-4BA5-BF20-175AE0FF52DA', @Published, @NewPublished, @RecordTitle, NULL;
 
 RETURN 1;
END
ELSE
BEGIN
 RETURN -99;
END