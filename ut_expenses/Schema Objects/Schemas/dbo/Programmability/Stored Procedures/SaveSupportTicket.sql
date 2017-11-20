CREATE PROCEDURE [dbo].[SaveSupportTicket]
	@SupportTicketID INT,
	@Subject NVARCHAR(255),
	@Description NVARCHAR(4000),
	@Status INT,
	@UserID INT, 
	@DelegateID INT,
	@SubAccountID INT
AS

DECLARE @currentUser INT;
DECLARE @recordTitle NVARCHAR(2000) = 'Support Ticket "' + @Subject + '"';
SET @currentUser = COALESCE(@DelegateID, @UserID);

IF (@SupportTicketID = 0)
BEGIN -- Insert New Record
	INSERT INTO [dbo].[SupportTickets]
	(
		[Owner],
		[Subject],
		[Description],
		[Status],
		[CreatedOn],
		[CreatedBy]
	)
	VALUES
	(
		@UserID,
		@Subject,
		@Description,
		@Status,
		GETUTCDATE(),
		@currentUser
	)
	
	SET @SupportTicketID = SCOPE_IDENTITY();
	
	EXEC addInsertEntryToAuditLog @UserID, @DelegateID, 182, @SupportTicketID, @recordTitle, @SubAccountID;
END

ELSE --Update Existing Record
BEGIN
	-- Create a backup of existing data
	DECLARE	@oldSubject NVARCHAR(255);
	DECLARE	@oldDescription NVARCHAR(4000);
	DECLARE	@oldStatus TINYINT;
	
	-- Create a backup of existing data
	SELECT	@oldSubject = [Subject],
			@oldDescription = [Description],
			@oldStatus = [Status]
	FROM	[SupportTickets]
	WHERE	[SupportTicketID] = @SupportTicketID;
	
	-- Perform the update
	UPDATE	[dbo].[SupportTickets]
	SET		[Subject] = @Subject,
			[Description] = @Description,
			[Status] = @Status,
			[ModifiedOn] = GETUTCDATE(),
			[ModifiedBy] = @currentUser
	WHERE	[SupportTicketID] = @SupportTicketID;

	-- Update the audit log
	IF (@oldSubject <> @Subject)
		EXEC addUpdateEntryToAuditLog @userid, @delegateID, 182, @SupportTicketID, 'D80DA1CD-89CF-46AC-AE73-AFCBC3722D3F', @oldSubject, @Subject, @recordTitle, @SubAccountID;

	IF (@oldDescription <> @Description)
		EXEC addUpdateEntryToAuditLog @userid, @delegateID, 182, @SupportTicketID, 'C056E55A-3D91-42DB-B0C2-097886D87E4E', @oldDescription, @Description, @recordTitle, @SubAccountID;
	
	IF (@oldSubject <> @Subject)
		EXEC addUpdateEntryToAuditLog @userid, @delegateID, 182, @SupportTicketID, '3C0D4144-1E46-4FDC-B49E-07793382BA8C', @oldStatus, @Status, @recordTitle, @SubAccountID;
END

RETURN @SupportTicketID;
GO