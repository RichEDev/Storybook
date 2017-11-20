



CREATE PROCEDURE [dbo].[deleteImportTemplateMappings]

@TemplateID int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	declare @title1 nvarchar(500);
	select @title1 = templateName FROM importTemplates WHERE templateID = @templateID;

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Import Template Mappings - ' + @title1);

	DELETE FROM importTemplateMappings WHERE TemplateID = @TemplateID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 27, @templateID, @recordTitle, null;
END



