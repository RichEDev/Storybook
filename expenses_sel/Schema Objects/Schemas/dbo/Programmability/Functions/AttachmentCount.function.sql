CREATE FUNCTION [dbo].[AttachmentCount] (@attArea INT, @refID INT)  
RETURNS INT AS  
BEGIN 

DECLARE @attachmentCount INT
SET @attachmentCount = (SELECT COUNT(*) FROM [attachments] WHERE [AttachmentArea] = @attArea AND [ReferenceNumber] = @refID)

RETURN(@attachmentCount)
 
END
