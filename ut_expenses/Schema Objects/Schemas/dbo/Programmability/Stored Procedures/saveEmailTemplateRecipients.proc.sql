


CREATE PROCEDURE [dbo].[saveEmailTemplateRecipients] 
@emailtemplateid int,
@sendertype tinyint,
@sender nvarchar(100),
@idofsender int,
@recipienttype tinyint,
@CUemployeeID INT,
@CUdelegateID INT

AS
BEGIN
	declare @title1 nvarchar(100);
	declare @recordTitle nvarchar(2000);
	select @title1 = templatename from emailTemplates where emailtemplateid = @emailtemplateid;
	set @recordTitle = (select @title1 + ' recipient');

	insert into emailTemplateRecipients (emailtemplateid, sendertype, sender, idofsender, recipienttype)
	values (@emailtemplateid, @sendertype, @sender, @idofsender, @recipienttype);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, @recordTitle, null;
END





 
