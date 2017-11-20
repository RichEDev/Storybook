CREATE PROCEDURE [dbo].[APIgetTemplates]
	@templateID int
AS
	select
		templateID, templateName, applicationType, isAutomated, NHSTrustID, createdOn, createdBy, modifiedOn, modifiedBy, SignOffOwnerFieldId, LineManagerFieldId
	from
		importTemplates
	where
		templateID = @templateID
RETURN 0
