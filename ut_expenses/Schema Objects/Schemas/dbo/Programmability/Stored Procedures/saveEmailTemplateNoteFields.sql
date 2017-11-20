CREATE PROCEDURE [dbo].[saveEmailTemplateNoteFields] 
@emailtemplateid int,
@fieldid uniqueidentifier,
@emailfieldtype tinyint,
@joinViaId    INT,
@CUemployeeID INT,
@CUdelegateID INT

AS
BEGIN
	declare @title1 nvarchar(100);
	declare @recordTitle nvarchar(2000);
	select @title1 = templatename from emailTemplates where emailtemplateid = @emailtemplateid;
	set @recordTitle = (select @title1 + ' notes field');

	insert into emailTemplateNoteFields (emailtemplateid, fieldid, emailfieldtype, joinViaId)
	values (@emailtemplateid, @fieldid, @emailfieldtype, @joinViaId);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, @recordTitle, null;
END