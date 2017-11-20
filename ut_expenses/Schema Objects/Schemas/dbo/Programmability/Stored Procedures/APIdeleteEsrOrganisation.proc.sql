CREATE PROCEDURE [dbo].[APIdeleteEsrOrganisation]
	@ESROrganisationId bigint 
	
AS
BEGIN
-- update foreign keys
	UPDATE ESROrganisations set ParentOrganisationId = null WHERE ParentOrganisationId = @ESROrganisationId;

-- delete record
	DELETE FROM ESROrganisations WHERE @ESROrganisationId = ESROrganisationId
END