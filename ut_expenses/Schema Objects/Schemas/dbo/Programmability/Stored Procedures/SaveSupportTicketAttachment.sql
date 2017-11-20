CREATE PROCEDURE [dbo].[SaveSupportTicketAttachment]
	@SupportTicketID INT,
	@Filename NVARCHAR(255),
	@FileData VARBINARY(MAX),
	@UserID INT, 
	@DelegateID INT,
	@SubAccountID INT
AS

DECLARE @SupportTicketAttachmentID INT;
DECLARE @currentUser INT;
DECLARE @recordTitle NVARCHAR(2000) = 'Attachment "' + @Filename + '" for Support Ticket "' + (SELECT [Subject] FROM SupportTickets WHERE SupportTicketId = @SupportTicketID) + '"';
SET @currentUser = COALESCE(@DelegateID, @UserID);

INSERT INTO [dbo].[SupportTicketAttachments]
(
	[SupportTicketId], 
	[Filename],
	[FileData],
	[CreatedOn],
	[CreatedBy]
)
VALUES
(
	@SupportTicketID,
	@Filename,
	@FileData,
	GETUTCDATE(),
	@currentUser
)
	
SET @SupportTicketAttachmentID = SCOPE_IDENTITY();
	
EXEC addInsertEntryToAuditLog @UserID, @DelegateID, 182, @SupportTicketAttachmentID, @recordTitle, @SubAccountID;

RETURN @SupportTicketAttachmentID;
GO
