
CREATE VIEW [dbo].[fields]
AS
SELECT     fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, 
                      lookuptable, lookupfield, useforlookup, workflowUpdate, workflowSearch, length, relabel, relabel_param, CAST(0 AS INT) AS fieldFrom, allowDuplicateChecking, 
                      classPropertyName, CAST(0 AS tinyint) AS FieldCategory, RelatedTable, IsForeignKey, associatedFieldForDuplicateChecking, DuplicateCheckingSource, DuplicateCheckingCalculation, friendlyNameTo, friendlyNameFrom
FROM         [$(targetMetabase)].dbo.fields_base
UNION ALL
SELECT     fieldid, tableid,  field,  fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist,
                       allowimport,  mandatory, amendedon, lookuptable, lookupfield, useforlookup, workflowUpdate, workflowSearch, 
					   length, CAST(0 as bit) Relabel, '' as relabel_Param, fieldFrom, allowDuplicateChecking, null as classPropertyName, FieldCategory, 
                      RelatedTable, IsForeignKey, CAST(null as Uniqueidentifier) as  associatedFieldForDuplicateChecking, DuplicateCheckingSource, DuplicateCheckingCalculation, '' as friendlyNameTo,'' as  friendlyNameFrom
FROM         CustomerFields
                      
GO


