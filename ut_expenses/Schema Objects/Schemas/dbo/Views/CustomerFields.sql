
CREATE VIEW [dbo].[CustomerFields]
	AS 
SELECT     dbo.[customEntityAttributes].fieldid, dbo.[customEntities].tableid, 'att' + CAST(dbo.[customEntityAttributes].attributeid AS NVARCHAR(10)) AS field, 
                      dbo.getFieldType(dbo.[customEntityAttributes].fieldtype, dbo.[customEntityAttributes].format) AS fieldtype, dbo.[customEntityAttributes].display_name AS description,
                       dbo.[customEntityAttributes].display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.[customEntityAttributes].is_key_field AS idfield, 
                      dbo.[customEntities].tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                      CASE dbo.[customEntityAttributes].fieldtype WHEN 4 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.[customEntityAttributes].mandatory, dbo.[customEntityAttributes].modifiedon AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) 
                      AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, dbo.[customEntityAttributes].maxlength AS length, CAST(0 AS BIT) 
                      AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedtable AS RelatedTable, case (dbo.[customEntityAttributes].fieldtype) when '22' then CAST(1 AS BIT) else CAST(0 AS BIT) END AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation
FROM         dbo.[customEntityAttributes] INNER JOIN
                      dbo.[customEntities] ON dbo.[customEntities].entityid = dbo.[customEntityAttributes].entityid
WHERE     ((dbo.[customEntityAttributes].relationshiptype = 1) OR
                      (dbo.[customEntityAttributes].relationshiptype IS NULL) AND (dbo.[customEntityAttributes].display_name <> 'Created On' AND 
                      dbo.[customEntityAttributes].display_name <> 'Created By' AND dbo.[customEntityAttributes].display_name <> 'Modified On' AND 
                      dbo.[customEntityAttributes].display_name <> 'Modified By')) AND dbo.[customEntityAttributes].fieldtype <> 9 
					  AND dbo.[customEntityAttributes].display_name <> 'GreenLight Currency' AND dbo.[customEntityAttributes].display_name <> 'Archived'
UNION ALL
SELECT     dbo.[customEntityAttributes].fieldid, dbo.[customEntities].tableid, 'GreenLightCurrency' AS field, 
                      N'CL' AS fieldtype, dbo.[customEntityAttributes].display_name AS description,
                       dbo.[customEntityAttributes].display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.[customEntityAttributes].is_key_field AS idfield, 
                      dbo.[customEntities].tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                       CAST(0 AS BIT) AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.[customEntityAttributes].mandatory, dbo.[customEntityAttributes].modifiedon AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) 
                      AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, dbo.[customEntityAttributes].maxlength AS length, CAST(0 AS BIT) 
                      AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedtable AS RelatedTable, case (dbo.[customEntityAttributes].fieldtype) when '22' then CAST(1 AS BIT) else CAST(0 AS BIT) END AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation
FROM         dbo.[customEntityAttributes] INNER JOIN
                      dbo.[customEntities] ON dbo.[customEntities].entityid = dbo.[customEntityAttributes].entityid
WHERE      dbo.[customEntityAttributes].fieldtype = 17 AND dbo.[customEntityAttributes].display_name = 'GreenLight Currency'
UNION ALL
SELECT     dbo.[customEntityAttributes].fieldid, dbo.[customEntities].tableid, 'Archived' AS field, 
                      dbo.getFieldType(dbo.[customEntityAttributes].fieldtype, dbo.[customEntityAttributes].format) AS fieldtype, dbo.[customEntityAttributes].display_name AS description,
                       dbo.[customEntityAttributes].display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.[customEntityAttributes].is_key_field AS idfield, 
                      dbo.[customEntities].tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                       CAST(0 AS BIT) AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.[customEntityAttributes].mandatory, dbo.[customEntityAttributes].modifiedon AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) 
                      AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, dbo.[customEntityAttributes].maxlength AS length, CAST(0 AS BIT) 
                      AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedtable AS RelatedTable, case (dbo.[customEntityAttributes].fieldtype) when '22' then CAST(1 AS BIT) else CAST(0 AS BIT) END AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation
FROM         dbo.[customEntityAttributes] INNER JOIN
                      dbo.[customEntities] ON dbo.[customEntities].entityid = dbo.[customEntityAttributes].entityid
WHERE      dbo.[customEntityAttributes].fieldtype = 5 AND dbo.[customEntityAttributes].display_name = 'Archived'
UNION ALL
SELECT     dbo.[customEntityAttributes].fieldid, dbo.[customEntities].tableid, REPLACE([customEntityAttributes].display_name,' ','') AS field, 
                      dbo.getFieldType(dbo.[customEntityAttributes].fieldtype, dbo.[customEntityAttributes].format) AS fieldtype, dbo.[customEntityAttributes].display_name AS description,
                       dbo.[customEntityAttributes].display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.[customEntityAttributes].is_key_field AS idfield, 
                      dbo.[customEntities].tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                      CASE dbo.[customEntityAttributes].fieldtype WHEN 4 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.[customEntityAttributes].mandatory, dbo.[customEntityAttributes].modifiedon AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) 
                      AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, dbo.[customEntityAttributes].maxlength AS length, CAST(0 AS BIT) 
                      AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedtable AS RelatedTable, CAST(0 AS BIT) AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation
FROM         dbo.[customEntityAttributes] INNER JOIN
                      dbo.[customEntities] ON dbo.[customEntities].entityid = dbo.[customEntityAttributes].entityid
WHERE     (((dbo.[customEntityAttributes].relationshiptype = 1) OR
                      (dbo.[customEntityAttributes].relationshiptype IS NULL)) AND (dbo.[customEntityAttributes].display_name = 'Created On' OR
                      dbo.[customEntityAttributes].display_name = 'Modified On'))
UNION ALL
SELECT     dbo.[customEntityAttributes].fieldid, dbo.[customEntities].tableid, REPLACE([customEntityAttributes].display_name,' ','') AS field, 
                      dbo.getFieldType(dbo.[customEntityAttributes].fieldtype, dbo.[customEntityAttributes].format) AS fieldtype, dbo.[customEntityAttributes].display_name AS description,
                       dbo.[customEntityAttributes].display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.[customEntityAttributes].is_key_field AS idfield, 
                      dbo.[customEntities].tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                      CAST(0 AS BIT) AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.[customEntityAttributes].mandatory, dbo.[customEntityAttributes].modifiedon AS amendedon, dbo.[customEntityAttributes].relatedtable AS lookuptable,
                          (SELECT     dbo.[tables].keyfield
                            FROM          dbo.[tables]
                            WHERE      dbo.[tables].tableid = dbo.[customEntityAttributes].relatedtable) AS lookupfield, CAST(0 AS BIT) AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, 
                      CAST(1 AS BIT) AS workflowSearch, dbo.[customEntityAttributes].maxlength AS length, CAST(0 AS BIT) AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, 
                      CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, relatedtable AS RelatedTable, CAST(1 AS BIT) AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation
FROM         dbo.[customEntityAttributes] INNER JOIN
                      dbo.[customEntities] ON dbo.[customEntities].entityid = dbo.[customEntityAttributes].entityid
WHERE     (dbo.[customEntityAttributes].relationshiptype = 1 AND dbo.[customEntityAttributes].fieldtype = 9) AND ([customEntityAttributes].display_name = 'Created By' OR
                      [customEntityAttributes].display_name = 'Modified By')
UNION ALL
SELECT     custom_entity_attributes_1.fieldid, custom_entity_attributes_1.relatedtable, 'att' + CAST(custom_entity_attributes_1.attributeid AS NVARCHAR(10)) AS field, 
                      dbo.getFieldType(custom_entity_attributes_1.fieldtype, custom_entity_attributes_1.format) AS fieldtype, custom_entity_attributes_1.display_name AS description, 
                      custom_entity_attributes_1.display_name AS comment, CAST(0 AS BIT) AS normalview, custom_entity_attributes_1.is_key_field AS idfield, 
                      custom_entities_1.tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                      CAST(0 AS BIT) AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      custom_entity_attributes_1.mandatory, custom_entity_attributes_1.modifiedon AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) 
                      AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, custom_entity_attributes_1.maxlength AS length, CAST(0 AS BIT) 
                      AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedtable AS RelatedTable, CAST(0 AS BIT) AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation
FROM         dbo.[customEntityAttributes] AS custom_entity_attributes_1 INNER JOIN
                      dbo.[customEntities] AS custom_entities_1 ON custom_entities_1.entityid = custom_entity_attributes_1.entityid
WHERE     (custom_entity_attributes_1.relationshiptype = 2) AND (custom_entity_attributes_1.fieldtype = 9) AND custom_entity_attributes_1.display_name <> 'Created On' AND 
                      custom_entity_attributes_1.display_name <> 'Created By' AND custom_entity_attributes_1.display_name <> 'Modified On' AND 
                      custom_entity_attributes_1.display_name <> 'Modified By' AND  custom_entity_attributes_1.display_name  <> 'Archived'
UNION ALL
SELECT     dbo.[customEntityAttributes].fieldid, dbo.[customEntities].tableid, 'att' + CAST(dbo.[customEntityAttributes].attributeid AS NVARCHAR(10)) AS field, 
                      dbo.getFieldType(dbo.[customEntityAttributes].fieldtype, dbo.[customEntityAttributes].format) AS fieldtype, dbo.[customEntityAttributes].display_name AS description,
                       dbo.[customEntityAttributes].display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.[customEntityAttributes].is_key_field AS idfield, 
                      dbo.[customEntities].tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                      CAST(0 AS BIT) AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.[customEntityAttributes].mandatory, dbo.[customEntityAttributes].modifiedon AS amendedon, dbo.[customEntityAttributes].relatedtable AS lookuptable,
                          (SELECT     dbo.[tables].keyfield
                            FROM          dbo.[tables]
                            WHERE      dbo.[tables].tableid = dbo.[customEntityAttributes].relatedtable) AS lookupfield, CAST(0 AS BIT) AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, 
                      CAST(1 AS BIT) AS workflowSearch, dbo.[customEntityAttributes].maxlength AS length, CAST(0 AS BIT) AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, 
                      CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, relatedtable AS RelatedTable, CAST(1 AS BIT) AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation
FROM         dbo.[customEntityAttributes] INNER JOIN
                      dbo.[customEntities] ON dbo.[customEntities].entityid = dbo.[customEntityAttributes].entityid
WHERE     dbo.[customEntityAttributes].relationshiptype = 1 AND dbo.[customEntityAttributes].fieldtype = 9 AND [customEntityAttributes].display_name <> 'Created On' AND 
                      [customEntityAttributes].display_name <> 'Created By' AND [customEntityAttributes].display_name <> 'Modified On' AND 
                      [customEntityAttributes].display_name <> 'Modified By' AND  [customEntityAttributes].display_name  <> 'Archived'
UNION ALL
SELECT     fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, 
                      lookuptable, lookupfield, useforlookup, workflowUpdate, workflowSearch, length, relabel, relabel_param, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) 
                      AS allowDuplicateChecking, NULL AS classPropertyName, FieldCategory, RelatedTable, IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation
FROM         dbo.[customFields]
UNION ALL
SELECT     fieldid, tableid, 'udf' + CAST(userdefineid AS NVARCHAR(10)) AS field, dbo.getFieldType(fieldtype, format) AS fieldtype, display_name AS description, 
                      display_name AS comment, CAST(1 AS BIT) AS normalview, CAST(0 AS BIT) AS idfield, dbo.getUserdefinedViewGroup(tableid, fieldtype) AS viewgroupid, CAST(0 AS BIT) 
                      AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, CASE userdefined.fieldtype WHEN 4 THEN CAST(1 AS BIT) 
                      ELSE CAST(0 AS BIT) END AS valuelist, CAST(1 AS BIT) AS allowimport, mandatory, ModifiedOn AS amendedon, NULL AS lookuptable, NULL AS lookupfield, 
                      CAST(0 AS BIT) AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, maxlength AS length, CAST(0 AS BIT) AS Expr1, NULL 
                      AS Expr2, CAST(2 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedTable AS RelatedTable, CASE fieldtype WHEN 9 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation
FROM         dbo.userdefined
UNION ALL
SELECT     fieldid, tableid, field, fieldtype, description, description AS comment, CAST(0 AS BIT) AS normalview, idfield, NULL AS viewgroupid, CAST(0 AS bit) AS genlist, 
                      CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS bit) AS Expr1, CAST(0 AS BIT) AS Expr2, CAST(0 AS BIT) AS Expr3, CAST(0 AS BIT) AS Expr4, NULL 
                      AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, 
                      0 AS length, CAST(0 AS BIT) AS Expr5, NULL AS Expr6, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, 
                      CAST(0 AS tinyint) AS FieldCategory, NULL AS RelatedTable, CAST(0 AS BIT) AS IsForeignKey, null as associatedFieldForDuplicateChecking, null as DuplicateCheckingSource, null as DuplicateCheckingCalculation
FROM         dbo.customEntityAttachmentFields

GO