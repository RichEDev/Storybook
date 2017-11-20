


CREATE PROCEDURE [dbo].[saveEmailTemplateSubjectFields] 
@emailtemplateid int,
@fieldid uniqueidentifier,
@emailfieldtype tinyint,
@CUemployeeID INT,
@CUdelegateID INT

AS
BEGIN
	declare @title1 nvarchar(100);
	declare @recordTitle nvarchar(2000);
	select @title1 = templatename from emailTemplates where emailtemplateid = @emailtemplateid;
	set @recordTitle = (select @title1 + ' subject field');

	insert into emailTemplateSubjectFields (emailtemplateid, fieldid, emailfieldtype)
	values (@emailtemplateid, @fieldid, @emailfieldtype);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, @recordTitle, null;
END





 
