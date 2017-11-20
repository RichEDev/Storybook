



CREATE PROCEDURE [dbo].[deleteImportTemplate]

@templateID int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	declare @title1 nvarchar(500);
	select @title1 = templateName FROM importTemplates WHERE templateID = @templateID;

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Import Template - ' + @title1);

	DELETE FROM importTemplates WHERE templateID = @templateID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 27, @templateID, @recordTitle, null;
END



