CREATE PROCEDURE [dbo].[DeleteKnowledgeCustomArticle]
 @KnowledgeCustomArticleId INT,
 @UserID INT, 
 @DelegateID INT
AS
BEGIN
 
DECLARE @RecordTitle NVARCHAR(MAX);
SET @RecordTitle = (SELECT Title FROM KnowledgeCustomArticles WHERE KnowledgeCustomArticleId = @KnowledgeCustomArticleId)

DELETE FROM [dbo].[KnowledgeCustomArticles] WHERE [KnowledgeCustomArticleId] = @KnowledgeCustomArticleId

EXEC addDeleteEntryToAuditLog @UserID, @DelegateID, 31, @KnowledgeCustomArticleId, @RecordTitle, null;

END
GO