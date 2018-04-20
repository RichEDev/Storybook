CREATE VIEW [dbo].[CustomerFields]
AS
SELECT
  dbo.[customentityattributes].fieldid,
  CASE 
    WHEN dbo.[customEntityAttributes].fieldtype = 9 AND dbo.[customEntityAttributes].relationshiptype = 2 AND dbo.[customEntityAttributes].display_name <> 'Created On' AND 
                      dbo.[customEntityAttributes].display_name <> 'Created By' AND dbo.[customEntityAttributes].display_name <> 'Modified On' AND 
                      dbo.[customEntityAttributes].display_name <> 'Modified By' AND  dbo.[customEntityAttributes].display_name  <> 'Archived' THEN 
					  dbo.[customEntityAttributes].relatedtable
    ELSE
  dbo.[customentities].tableid
  END AS tableid,
  CASE dbo.[customEntityAttributes].display_name
    WHEN 'GreenLight Currency' THEN 'GreenLightCurrency'
    WHEN 'Created On' THEN REPLACE(dbo.[customEntityAttributes].display_name, ' ', '')
    WHEN 'Created By' THEN REPLACE(dbo.[customEntityAttributes].display_name, ' ', '')
    WHEN 'Modified On' THEN REPLACE(dbo.[customEntityAttributes].display_name, ' ', '')
    WHEN 'Modified By' THEN REPLACE(dbo.[customEntityAttributes].display_name, ' ', '')
	WHEN 'Archived' THEN CASE dbo.[customEntityAttributes].fieldtype WHEN 5 THEN 'Archived' ELSE 'att' + CAST(dbo.[customentityattributes].attributeid AS nvarchar(10)) END
    ELSE 'att' + CAST(dbo.[customEntityAttributes].attributeid AS nvarchar(10))
  END AS field,
  CASE dbo.[customEntityAttributes].fieldtype
    WHEN '17' THEN N'CL'	
    ELSE dbo.getFieldType(dbo.[customEntityAttributes].fieldtype, dbo.[customEntityAttributes].format)
  END AS fieldtype,
  dbo.[customEntityAttributes].display_name AS description,
  dbo.[customEntityAttributes].display_name AS comment,
  CAST(0 AS bit) AS normalview,
  dbo.[customEntityAttributes].is_key_field AS idfield,
  dbo.[customentities].tableid AS viewgroupid,
  CAST(0 AS BIT) AS genlist,
  CAST(50 AS INT) AS width,
  CAST(0 AS BIT) AS cantotal,
  CAST(0 AS BIT) AS printout,
  CASE dbo.[customentityattributes].fieldtype
    WHEN 4 THEN CAST(1 AS BIT)
    ELSE CAST(0 AS BIT)
  END AS valuelist,
  CAST(1 AS BIT) AS allowimport,
  dbo.[customEntityAttributes].mandatory,
  dbo.[customEntityAttributes].modifiedon AS amendedon,
  CASE 
	WHEN ((dbo.[customEntityAttributes].fieldtype = 9 AND dbo.[customEntityAttributes].relationshiptype = 2 AND dbo.[customEntityAttributes].display_name <> 'Created On' AND 
                      dbo.[customEntityAttributes].display_name <> 'Created By' AND dbo.[customEntityAttributes].display_name <> 'Modified On' AND 
                      dbo.[customEntityAttributes].display_name <> 'Modified By' AND  dbo.[customEntityAttributes].display_name  <> 'Archived') 
					  OR (dbo.[customEntityAttributes].fieldtype = 17 AND dbo.[customEntityAttributes].display_name = 'GreenLight Currency')
					  OR (dbo.[customEntityAttributes].fieldtype = 5 AND dbo.[customEntityAttributes].display_name = 'Archived')
					  OR (dbo.[customEntityAttributes].fieldtype = 22 AND dbo.[customEntityAttributes].display_name = 'Attachment'))
	THEN null
	ELSE dbo.[customEntityAttributes].relatedtable
  END AS lookuptable,
  CASE 
	WHEN ((dbo.[customEntityAttributes].fieldtype = 9 AND dbo.[customEntityAttributes].relationshiptype = 2 AND dbo.[customEntityAttributes].display_name <> 'Created On' AND 
                      dbo.[customEntityAttributes].display_name <> 'Created By' AND dbo.[customEntityAttributes].display_name <> 'Modified On' AND 
                      dbo.[customEntityAttributes].display_name <> 'Modified By' AND  dbo.[customEntityAttributes].display_name  <> 'Archived') 
					  OR (dbo.[customEntityAttributes].fieldtype = 17 AND dbo.[customEntityAttributes].display_name = 'GreenLight Currency')
					  OR (dbo.[customEntityAttributes].fieldtype = 5 AND dbo.[customEntityAttributes].display_name = 'Archived')
					  OR (dbo.[customEntityAttributes].fieldtype = 22 AND dbo.[customEntityAttributes].display_name = 'Attachment'))
	THEN null
	ELSE dbo.[tables].keyfield
  END AS lookupfield,
  CAST(0 AS BIT) AS useforlookup,
  CAST(1 AS BIT) AS workflowUpdate,
  CAST(1 AS BIT) AS workflowSearch,
  dbo.[customentityattributes].maxlength AS length,
  CAST(0 AS BIT) AS Expr1,
  NULL AS Expr2,
  CAST(1 AS INT) AS fieldFrom,
  CAST(0 AS BIT) AS allowDuplicateChecking,
  NULL AS classPropertyName,
  CAST(0 AS TINYINT) AS FieldCategory,
  dbo.[customEntityAttributes].relatedtable AS RelatedTable,
  CASE (dbo.[customEntityAttributes].fieldtype)
    WHEN '22' THEN CAST(1 AS BIT)
    WHEN '9' THEN CASE dbo.[customEntityAttributes].relationshiptype
        WHEN 1 THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
      END
	ELSE CAST(0 AS BIT)
  END AS IsForeignKey,
  NULL AS associatedFieldForDuplicateChecking,
  NULL AS DuplicateCheckingSource,
  NULL AS DuplicateCheckingCalculation,
  [Encrypted]
FROM dbo.[customEntityAttributes]
INNER JOIN dbo.[customentities]
  ON dbo.[customentities].entityid = dbo.[customEntityAttributes].entityid
LEFT JOIN tables
  ON tables.tableid = dbo.[customEntityAttributes].relatedtable
UNION
SELECT
  fieldid,
  tableid,
  field,
  fieldtype,
  description,
  description AS comment,
  CAST(0 AS BIT) AS normalview,
  idfield,
  NULL AS viewgroupid,
  CAST(0 AS BIT) AS genlist,
  CAST(50 AS INT) AS width,
  CAST(0 AS BIT) AS cantotal,
  CAST(0 AS BIT) AS Expr1,
  CAST(0 AS BIT) AS Expr2,
  CAST(0 AS BIT) AS Expr3,
  CAST(0 AS BIT) AS Expr4,
  NULL AS amendedon,
  NULL AS lookuptable,
  NULL AS lookupfield,
  CAST(0 AS BIT) AS useforlookup,
  CAST(1 AS BIT) AS workflowUpdate,
  CAST(1 AS BIT) AS workflowSearch,
  0 AS length,
  CAST(0 AS BIT) AS Expr5,
  NULL AS Expr6,
  CAST(1 AS INT) AS fieldFrom,
  CAST(0 AS BIT) AS allowDuplicateChecking,
  NULL AS classPropertyName,
  CAST(0 AS TINYINT) AS FieldCategory,
  NULL AS RelatedTable,
  CAST(0 AS BIT) AS IsForeignKey,
  NULL AS associatedFieldForDuplicateChecking,
  NULL AS DuplicateCheckingSource,
  NULL AS DuplicateCheckingCalculation,
  CAST(0 AS BIT) AS [Encrypted]
FROM dbo.customEntityAttachmentFields
UNION ALL
SELECT
  fieldid,
  tableid,
  field,
  fieldtype,
  description,
  comment,
  normalview,
  idfield,
  viewgroupid,
  genlist,
  width,
  cantotal,
  printout,
  valuelist,
  allowimport,
  mandatory,
  amendedon,
  lookuptable,
  lookupfield,
  useforlookup,
  workflowUpdate,
  workflowSearch,
  length,
  relabel,
  relabel_param,
  CAST(1 AS INT) AS fieldFrom,
  CAST(0 AS BIT) AS allowDuplicateChecking,
  NULL AS classPropertyName,
  FieldCategory,
  RelatedTable,
  IsForeignKey,
  NULL AS associatedFieldForDuplicateChecking,
  NULL AS DuplicateCheckingSource,
  NULL AS DuplicateCheckingCalculation,
  CAST(0 AS BIT) AS [Encrypted]
FROM dbo.[customFields]
UNION ALL
SELECT
  fieldid,
  tableid,
  'udf' + CAST(userdefineid AS NVARCHAR(10)) AS field,
  dbo.getFieldType(fieldtype, format) AS fieldtype,
  display_name AS description,
  display_name AS comment,
  CAST(1 AS BIT) AS normalview,
  CAST(0 AS BIT) AS idfield,
  dbo.getUserdefinedViewGroup(tableid, fieldtype) AS viewgroupid,
  CAST(0 AS BIT) AS genlist,
  CAST(50 AS INT) AS width,
  CAST(0 AS BIT) AS cantotal,
  CAST(0 AS BIT) AS printout,
  CASE userdefined.fieldtype
    WHEN 4 THEN CAST(1 AS BIT)
    ELSE CAST(0 AS BIT)
  END AS valuelist,
  CAST(1 AS BIT) AS allowimport,
  mandatory,
  ModifiedOn AS amendedon,
  NULL AS lookuptable,
  NULL AS lookupfield,
  CAST(0 AS BIT) AS useforlookup,
  CAST(1 AS BIT) AS workflowUpdate,
  CAST(1 AS BIT) AS workflowSearch,
  maxlength AS length,
  CAST(0 AS BIT) AS Expr1,
  NULL AS Expr2,
  CAST(2 AS INT) AS fieldFrom,
  CAST(0 AS BIT) AS allowDuplicateChecking,
  NULL AS classPropertyName,
  CAST(0 AS TINYINT) AS FieldCategory,
  relatedTable AS RelatedTable,
  CASE fieldtype
    WHEN 9 THEN CAST(1 AS BIT)
    ELSE CAST(0 AS BIT)
  END AS IsForeignKey,
  NULL AS associatedFieldForDuplicateChecking,
  NULL AS DuplicateCheckingSource,
  NULL AS DuplicateCheckingCalculation,
  [Encrypted]
FROM dbo.userdefined

GO