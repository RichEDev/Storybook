CREATE procedure [dbo].[deleteDocumentTemplate]
@documentid int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @recordTitle nvarchar(2000);
	select @recordTitle = doc_name from document_templates where documentid = @documentid;

	delete from document_template_data where documentid = @documentid
	delete from document_templates where documentid = @documentid

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 103, @documentid, @recordTitle, null;
end
