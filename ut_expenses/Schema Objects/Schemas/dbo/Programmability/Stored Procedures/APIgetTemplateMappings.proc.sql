CREATE PROCEDURE [dbo].[APIgetTemplateMappings]
	@templateMappingID int = 0
	
AS
IF @templateMappingID = 0
	BEGIN
	SELECT     importTemplateMappings.templateMappingID, importTemplateMappings.templateID, importTemplateMappings.fieldID, importTemplateMappings.destinationField, 
                      importTemplateMappings.columnRef, importTemplateMappings.importElementType, importTemplateMappings.mandatory, importTemplateMappings.dataType, 
                      importTemplateMappings.lookupTable, importTemplateMappings.matchField, importTemplateMappings.overridePrimaryKey, importTemplateMappings.importField, 
                      esrTrusts.trustVPD, fields.tableid
FROM         importTemplateMappings INNER JOIN
                      importTemplates ON importTemplateMappings.templateID = importTemplates.templateID INNER JOIN
                      esrTrusts ON importTemplates.NHSTrustID = esrTrusts.trustID  AND ESRTRUSTS.ESRVersionNumber = 2 INNER JOIN
                      fields ON importTemplateMappings.fieldID = fields.fieldid
	END
ELSE
	BEGIN
	SELECT     importTemplateMappings.templateMappingID, importTemplateMappings.templateID, importTemplateMappings.fieldID, importTemplateMappings.destinationField, 
                      importTemplateMappings.columnRef, importTemplateMappings.importElementType, importTemplateMappings.mandatory, importTemplateMappings.dataType, 
                      importTemplateMappings.lookupTable, importTemplateMappings.matchField, importTemplateMappings.overridePrimaryKey, importTemplateMappings.importField, 
                      esrTrusts.trustVPD, fields.tableid
FROM         importTemplateMappings INNER JOIN
                      importTemplates ON importTemplateMappings.templateID = importTemplates.templateID INNER JOIN
                      esrTrusts ON importTemplates.NHSTrustID = esrTrusts.trustID  AND ESRTRUSTS.ESRVersionNumber = 2 INNER JOIN
                      fields ON importTemplateMappings.fieldID = fields.fieldid
	WHERE templateMappingID = @templateMappingID
	END
RETURN 0