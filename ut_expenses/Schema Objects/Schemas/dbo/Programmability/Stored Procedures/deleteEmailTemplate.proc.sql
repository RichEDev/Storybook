



CREATE PROCEDURE [dbo].[deleteEmailTemplate] (@emailtemplateid int,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	declare @recordtitle nvarchar(2000);
	select @recordtitle = templatename from emailTemplates where emailtemplateid = @emailtemplateid;

	delete from emailTemplates where emailtemplateid = @emailtemplateid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, @recordTitle, null;	


	DECLARE @attachmentID int;
	
	declare loop_cursor CURSOR FOR
	SELECT attachmentID FROM emailTemplate_attachments WHERE id = @emailTemplateID
	open loop_cursor
	fetch next from loop_cursor into @attachmentID
	while @@FETCH_STATUS = 0
	begin
		delete from emailTemplate_attachmentData where attachmentID = @attachmentID
	fetch next from loop_cursor into @attachmentID
	end
	close loop_cursor
	deallocate loop_cursor	

END
