CREATE FUNCTION [dbo].[CheckAttachmentAccess] (@attachmentID INT, @userID INT)  
RETURNS int AS  
BEGIN
DECLARE @AccessCount INT

SET @AccessCount = (
      SELECT COUNT(*) AS [AccessCount] FROM [attachment_audience] WHERE [attachmentId] = @attachmentID AND 
      (
            ([audienceType] = 0 AND [accessId] = @userID) 
            OR 
            ([audienceType] = 1 AND [accessId] IN 
                  (SELECT [teamid] FROM [teams] WHERE @userID IN
                        (SELECT [employeeid] FROM [teamemps] WHERE [accessId] = [teamid])
                  )
            )
      )
)

IF(@AccessCount = 0)
BEGIN
      -- Check that any restrictions exist
      DECLARE @tmpCount INT
      SET @tmpCount = (SELECT COUNT(*) FROM [attachment_audience] WHERE [attachmentId] = @attachmentID)
      IF(@tmpCount = 0)
      BEGIN
            -- no restrictions exist, so permit
            SET @AccessCount = 999
      END
END

RETURN(@AccessCount) 
END

