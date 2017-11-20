CREATE PROCEDURE [dbo].[SaveKnowledgeCustomArticle]
 @KnowledgeCustomArticleId INT,
 @Title NVARCHAR(255),
 @ProductCategory NVARCHAR(255),
 @Summary NVARCHAR(255),
 @Body NVARCHAR(MAX),
 @UserID INT, 
 @DelegateID INT
AS
 
-- Duplicate check
IF EXISTS (SELECT KnowledgeCustomArticleId FROM KnowledgeCustomArticles WHERE KnowledgeCustomArticleId <> @KnowledgeCustomArticleId AND Title = @Title)
BEGIN
 RETURN -1;
END

DECLARE @RecordTitle NVARCHAR(MAX);

IF (@KnowledgeCustomArticleId = 0)
BEGIN -- Insert New Record
 
 INSERT INTO [dbo].[KnowledgeCustomArticles]
 (
   [Title],
   [ProductCategory],
   [Summary],
   [Body],
   [CreatedBy],
   [CreatedOn]
 )
 VALUES
 (
   @Title,
   @ProductCategory,
   @Summary,
   @Body,
   @UserID,
   GETUTCDATE()
 )

 SET @KnowledgeCustomArticleId = SCOPE_IDENTITY();

 EXEC addInsertEntryToAuditLog @userid, @delegateID, 31, @KnowledgeCustomArticleId, @Title, null;

END
ELSE --Update Existing Record
BEGIN

 -- Create a backup of existing data
 DECLARE @oldTitle NVARCHAR(255);
 DECLARE @oldProductCategory NVARCHAR(255);
 DECLARE @oldSummary NVARCHAR(255);
 DECLARE @oldBody NVARCHAR(MAX);

 SELECT  @oldTitle = [Title],
   @oldProductCategory = [ProductCategory],
   @oldSummary = [Summary],
   @oldBody = [Body]
 FROM [KnowledgeCustomArticles]
 WHERE [KnowledgeCustomArticleId] = @KnowledgeCustomArticleId;
  
 SET @RecordTitle = @Title;

 -- Perform the update
 UPDATE [dbo].[KnowledgeCustomArticles] 
 SET [Title] = @Title, [ProductCategory] = @ProductCategory, [Summary] = @Summary, [Body] = @Body 
 WHERE [KnowledgeCustomArticleId] = @KnowledgeCustomArticleId;
 
 -- Update the audit log 
 IF (@oldTitle <> @Title)
 EXEC addUpdateEntryToAuditLog @userid, @delegateID, 31, @KnowledgeCustomArticleId, '65EB4A84-3303-4EC2-9777-294995FA7FE0', @oldTitle, @Title, @RecordTitle, NULL;

 IF (@oldProductCategory <> @ProductCategory)
    EXEC addUpdateEntryToAuditLog @userid, @delegateID, 31, @KnowledgeCustomArticleId, '0670EDD9-B163-4CC7-9CB2-677C4CD024BB', @oldProductCategory, @ProductCategory, @RecordTitle, NULL;

 IF (@oldSummary <> @Summary)
 EXEC addUpdateEntryToAuditLog @userid, @delegateID, 31, @KnowledgeCustomArticleId, '8EE181CE-A67B-4099-BAB2-78F3B7D2F96A', @oldSummary, @Summary, @RecordTitle, NULL;

 IF (@oldBody <> @Body)
    EXEC addUpdateEntryToAuditLog @userid, @delegateID, 31, @KnowledgeCustomArticleId, '16DE32E1-0544-42B1-A695-D80D693470D4', @oldBody, @Body, @RecordTitle, NULL;

 
 RETURN @KnowledgeCustomArticleId;
END
GO