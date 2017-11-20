CREATE VIEW [dbo].[fields]
AS
SELECT     fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, 
                      lookuptable, lookupfield, useforlookup, workflowUpdate, workflowSearch, length, relabel, relabel_param, CAST(0 AS INT) AS fieldFrom, allowDuplicateChecking, 
                      classPropertyName, CAST(0 AS tinyint) AS FieldCategory, NULL AS RelatedTable
FROM         [$(metabaseExpenses)].dbo.fields_base
UNION ALL
SELECT     dbo.custom_entity_attributes.fieldid, dbo.custom_entities.tableid, 'att' + CAST(dbo.custom_entity_attributes.attributeid AS NVARCHAR(10)) AS field, 
                      dbo.getFieldType(dbo.custom_entity_attributes.fieldtype, dbo.custom_entity_attributes.format) AS fieldtype, dbo.custom_entity_attributes.display_name AS description,
                       dbo.custom_entity_attributes.display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.custom_entity_attributes.is_key_field AS idfield, 
                      dbo.custom_entities.tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                      CASE dbo.custom_entity_attributes.fieldtype WHEN 4 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.custom_entity_attributes.mandatory, dbo.custom_entity_attributes.modifiedon AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) 
                      AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, dbo.custom_entity_attributes.maxlength AS length, CAST(0 AS BIT) 
                      AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedtable AS RelatedTable
FROM         dbo.custom_entity_attributes INNER JOIN
                      dbo.custom_entities ON dbo.custom_entities.entityid = dbo.custom_entity_attributes.entityid
WHERE     ((dbo.custom_entity_attributes.relationshiptype = 1) OR
                      (dbo.custom_entity_attributes.relationshiptype IS NULL) AND (dbo.custom_entity_attributes.attribute_name <> 'CreatedOn' AND 
                      dbo.custom_entity_attributes.attribute_name <> 'CreatedBy' AND dbo.custom_entity_attributes.attribute_name <> 'ModifiedOn' AND 
                      dbo.custom_entity_attributes.attribute_name <> 'ModifiedBy')) AND dbo.custom_entity_attributes.fieldtype <> 9 AND dbo.custom_entity_attributes.attribute_name <> 'EntityCurrency'
UNION ALL
SELECT     dbo.custom_entity_attributes.fieldid, dbo.custom_entities.tableid, dbo.custom_entity_attributes.attribute_name AS field, 
                      N'CL' AS fieldtype, dbo.custom_entity_attributes.display_name AS description,
                       dbo.custom_entity_attributes.display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.custom_entity_attributes.is_key_field AS idfield, 
                      dbo.custom_entities.tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                       CAST(0 AS BIT) AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.custom_entity_attributes.mandatory, dbo.custom_entity_attributes.modifiedon AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) 
                      AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, dbo.custom_entity_attributes.maxlength AS length, CAST(0 AS BIT) 
                      AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedtable AS RelatedTable
FROM         dbo.custom_entity_attributes INNER JOIN
                      dbo.custom_entities ON dbo.custom_entities.entityid = dbo.custom_entity_attributes.entityid
WHERE      dbo.custom_entity_attributes.fieldtype = 17 AND dbo.custom_entity_attributes.attribute_name = 'EntityCurrency'
UNION ALL
SELECT     dbo.custom_entity_attributes.fieldid, dbo.custom_entities.tableid, custom_entity_attributes.attribute_name AS field, 
                      dbo.getFieldType(dbo.custom_entity_attributes.fieldtype, dbo.custom_entity_attributes.format) AS fieldtype, dbo.custom_entity_attributes.display_name AS description,
                       dbo.custom_entity_attributes.display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.custom_entity_attributes.is_key_field AS idfield, 
                      dbo.custom_entities.tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                      CASE dbo.custom_entity_attributes.fieldtype WHEN 4 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.custom_entity_attributes.mandatory, dbo.custom_entity_attributes.modifiedon AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) 
                      AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, dbo.custom_entity_attributes.maxlength AS length, CAST(0 AS BIT) 
                      AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedtable AS RelatedTable
FROM         dbo.custom_entity_attributes INNER JOIN
                      dbo.custom_entities ON dbo.custom_entities.entityid = dbo.custom_entity_attributes.entityid
WHERE     (((dbo.custom_entity_attributes.relationshiptype = 1) OR
                      (dbo.custom_entity_attributes.relationshiptype IS NULL)) AND (dbo.custom_entity_attributes.attribute_name = 'CreatedOn' OR
                      dbo.custom_entity_attributes.attribute_name = 'ModifiedOn'))
UNION ALL
SELECT     dbo.custom_entity_attributes.fieldid, dbo.custom_entities.tableid, custom_entity_attributes.attribute_name AS field, 
                      dbo.getFieldType(dbo.custom_entity_attributes.fieldtype, dbo.custom_entity_attributes.format) AS fieldtype, dbo.custom_entity_attributes.display_name AS description,
                       dbo.custom_entity_attributes.display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.custom_entity_attributes.is_key_field AS idfield, 
                      dbo.custom_entities.tableid AS viewgroupid, CAST(1 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                      CASE dbo.custom_entity_attributes.fieldtype WHEN 4 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.custom_entity_attributes.mandatory, dbo.custom_entity_attributes.modifiedon AS amendedon, dbo.custom_entity_attributes.relatedtable AS lookuptable,
                          (SELECT     dbo.[tables].keyfield
                            FROM          dbo.[tables]
                            WHERE      dbo.[tables].tableid = dbo.custom_entity_attributes.relatedtable) AS lookupfield, CAST(0 AS BIT) AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, 
                      CAST(1 AS BIT) AS workflowSearch, dbo.custom_entity_attributes.maxlength AS length, CAST(0 AS BIT) AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, 
                      CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, relatedtable AS RelatedTable
FROM         dbo.custom_entity_attributes INNER JOIN
                      dbo.custom_entities ON dbo.custom_entities.entityid = dbo.custom_entity_attributes.entityid
WHERE     (dbo.custom_entity_attributes.relationshiptype = 1 AND dbo.custom_entity_attributes.fieldtype = 9) AND (custom_entity_attributes.attribute_name = 'CreatedBy' OR
                      custom_entity_attributes.attribute_name = 'ModifiedBy')
UNION ALL
SELECT     custom_entity_attributes_1.fieldid, custom_entity_attributes_1.relatedtable, 'att' + CAST(custom_entity_attributes_1.attributeid AS NVARCHAR(10)) AS field, 
                      dbo.getFieldType(custom_entity_attributes_1.fieldtype, custom_entity_attributes_1.format) AS fieldtype, custom_entity_attributes_1.display_name AS description, 
                      custom_entity_attributes_1.display_name AS comment, CAST(0 AS BIT) AS normalview, custom_entity_attributes_1.is_key_field AS idfield, 
                      custom_entities_1.tableid AS viewgroupid, CAST(0 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                      CASE custom_entity_attributes_1.fieldtype WHEN 4 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      custom_entity_attributes_1.mandatory, custom_entity_attributes_1.modifiedon AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) 
                      AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, custom_entity_attributes_1.maxlength AS length, CAST(0 AS BIT) 
                      AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedtable AS RelatedTable
FROM         dbo.custom_entity_attributes AS custom_entity_attributes_1 INNER JOIN
                      dbo.custom_entities AS custom_entities_1 ON custom_entities_1.entityid = custom_entity_attributes_1.entityid
WHERE     (custom_entity_attributes_1.relationshiptype = 2) AND (custom_entity_attributes_1.fieldtype = 9) AND custom_entity_attributes_1.attribute_name <> 'CreatedOn' AND 
                      custom_entity_attributes_1.attribute_name <> 'CreatedBy' AND custom_entity_attributes_1.attribute_name <> 'ModifiedOn' AND 
                      custom_entity_attributes_1.attribute_name <> 'ModifiedBy'
UNION ALL
SELECT     dbo.custom_entity_attributes.fieldid, dbo.custom_entities.tableid, 'att' + CAST(dbo.custom_entity_attributes.attributeid AS NVARCHAR(10)) AS field, 
                      dbo.getFieldType(dbo.custom_entity_attributes.fieldtype, dbo.custom_entity_attributes.format) AS fieldtype, dbo.custom_entity_attributes.display_name AS description,
                       dbo.custom_entity_attributes.display_name AS comment, CAST(0 AS BIT) AS normalview, dbo.custom_entity_attributes.is_key_field AS idfield, 
                      dbo.custom_entities.tableid AS viewgroupid, CAST(1 AS BIT) AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, 
                      CASE dbo.custom_entity_attributes.fieldtype WHEN 4 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS valuelist, CAST(1 AS BIT) AS allowimport, 
                      dbo.custom_entity_attributes.mandatory, dbo.custom_entity_attributes.modifiedon AS amendedon, dbo.custom_entity_attributes.relatedtable AS lookuptable,
                          (SELECT     dbo.[tables].keyfield
                            FROM          dbo.[tables]
                            WHERE      dbo.[tables].tableid = dbo.custom_entity_attributes.relatedtable) AS lookupfield, CAST(0 AS BIT) AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, 
                      CAST(1 AS BIT) AS workflowSearch, dbo.custom_entity_attributes.maxlength AS length, CAST(0 AS BIT) AS Expr1, NULL AS Expr2, CAST(1 AS INT) AS fieldFrom, 
                      CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, relatedtable AS RelatedTable
FROM         dbo.custom_entity_attributes INNER JOIN
                      dbo.custom_entities ON dbo.custom_entities.entityid = dbo.custom_entity_attributes.entityid
WHERE     dbo.custom_entity_attributes.relationshiptype = 1 AND dbo.custom_entity_attributes.fieldtype = 9 AND custom_entity_attributes.attribute_name <> 'CreatedOn' AND 
                      custom_entity_attributes.attribute_name <> 'CreatedBy' AND custom_entity_attributes.attribute_name <> 'ModifiedOn' AND 
                      custom_entity_attributes.attribute_name <> 'ModifiedBy'
UNION ALL
SELECT     fieldid, tableid, field, fieldtype, description, comment, normalview, idfield, viewgroupid, genlist, width, cantotal, printout, valuelist, allowimport, mandatory, amendedon, 
                      lookuptable, lookupfield, useforlookup, workflowUpdate, workflowSearch, length, relabel, relabel_param, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) 
                      AS allowDuplicateChecking, NULL AS classPropertyName, FieldCategory, NULL AS RelatedTable
FROM         dbo.custom_fields
UNION ALL
SELECT     fieldid, tableid, 'udf' + CAST(userdefineid AS NVARCHAR(10)) AS field, dbo.getFieldType(fieldtype, format) AS fieldtype, display_name AS description, 
                      display_name AS comment, CAST(1 AS BIT) AS normalview, CAST(0 AS BIT) AS idfield, dbo.getUserdefinedViewGroup(tableid, fieldtype) AS viewgroupid, CAST(0 AS BIT) 
                      AS genlist, CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS BIT) AS printout, CASE userdefined.fieldtype WHEN 4 THEN CAST(1 AS BIT) 
                      ELSE CAST(0 AS BIT) END AS valuelist, CAST(1 AS BIT) AS allowimport, mandatory, ModifiedOn AS amendedon, NULL AS lookuptable, NULL AS lookupfield, 
                      CAST(0 AS BIT) AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, maxlength AS length, CAST(0 AS BIT) AS Expr1, NULL 
                      AS Expr2, CAST(2 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, CAST(0 AS tinyint) AS FieldCategory, 
                      relatedTable AS RelatedTable
FROM         dbo.userdefined
UNION ALL
SELECT     fieldid, tableid, field, fieldtype, description, description AS comment, CAST(0 AS BIT) AS normalview, idfield, NULL AS viewgroupid, CAST(0 AS bit) AS genlist, 
                      CAST(50 AS INT) AS width, CAST(0 AS BIT) AS cantotal, CAST(0 AS bit) AS Expr1, CAST(0 AS BIT) AS Expr2, CAST(0 AS BIT) AS Expr3, CAST(0 AS BIT) AS Expr4, NULL 
                      AS amendedon, NULL AS lookuptable, NULL AS lookupfield, CAST(0 AS BIT) AS useforlookup, CAST(1 AS BIT) AS workflowUpdate, CAST(1 AS BIT) AS workflowSearch, 
                      0 AS length, CAST(0 AS BIT) AS Expr5, NULL AS Expr6, CAST(1 AS INT) AS fieldFrom, CAST(0 AS BIT) AS allowDuplicateChecking, NULL AS classPropertyName, 
                      CAST(0 AS tinyint) AS FieldCategory, NULL AS RelatedTable
FROM         dbo.customEntityAttachmentFields
