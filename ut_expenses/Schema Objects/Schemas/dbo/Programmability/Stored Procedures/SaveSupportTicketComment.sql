CREATE PROCEDURE [dbo].[SaveSupportTicketComment]
	@SupportTicketID INT,
	@Body NVARCHAR(4000),
	@UserID INT, 
	@DelegateID INT,
	@SubAccountID INT
AS

DECLARE @SupportTicketCommentID INT;
DECLARE @currentUser INT;
DECLARE @recordTitle NVARCHAR(2000) = 'Comment for Support Ticket "' + (SELECT [Subject] FROM SupportTickets WHERE SupportTicketId = @SupportTicketID) + '"';
SET @currentUser = COALESCE(@DelegateID, @UserID);

INSERT INTO [dbo].[SupportTicketComments]
(
	[SupportTicketId],
	[Body],
	[CreatedOn],
	[CreatedBy]
)
VALUES
(
	@SupportTicketID,
	@Body,
	GETUTCDATE(),
	@currentUser
)
	
SET @SupportTicketCommentID = SCOPE_IDENTITY();
	
EXEC addInsertEntryToAuditLog @UserID, @DelegateID, 182, @SupportTicketCommentID, @recordTitle, @SubAccountID;

RETURN @SupportTicketCommentID;
GO
