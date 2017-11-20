



CREATE PROCEDURE [dbo].[deleteEmailTemplateAssociatedValues] (@emailtemplateid int,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'All associated values for template id ' + CAST(@emailtemplateid as nvarchar(20)));

	delete from emailTemplateSubjectFields where emailtemplateid = @emailtemplateid;
	delete from emailTemplateBodyFields where emailtemplateid = @emailtemplateid;
	delete from emailTemplateRecipients where emailtemplateid = @emailtemplateid;
	delete from emailTemplate_Attachments where id = @emailtemplateid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, @recordTitle, null;
END
