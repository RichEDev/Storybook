CREATE procedure [dbo].[deleteDocumentMapping]
@mappingId int
AS
BEGIN
	UPDATE document_mappings 
		SET isMergePart = 0, 
			modifiedOn = getdate()
		WHERE mappingid IN (SELECT table_mappingid FROM document_mappings_mergetable WHERE mappingid = @mappingId)
	
	DELETE FROM document_mappings_mergetable WHERE mappingid = @mappingId
	DELETE FROM document_mappings where mappingid = @mappingId
	
END
