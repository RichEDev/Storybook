CREATE VIEW [dbo].[CustomerFields]
AS
SELECT
  dbo.[customentityattributes].fieldid,
  dbo.[customentities].tableid,
  CASE dbo.[customentityattributes].display_name
    WHEN 'GreenLight Currency' THEN 'GreenLightCurrency'
    WHEN 'Created On' THEN REPLACE([customentityattributes].display_name, ' ', '')
    WHEN 'Created By' THEN REPLACE([customentityattributes].display_name, ' ', '')
    WHEN 'Modified On' THEN REPLACE([customentityattributes].display_name, ' ', '')
    WHEN 'Modified By' THEN REPLACE([customentityattributes].display_name, ' ', '')
	WHEN 'Archived' THEN CASE dbo.[customentityattributes].fieldtype WHEN 5 THEN 'Archived' ELSE 'att' + CAST(dbo.[customentityattributes].attributeid AS nvarchar(10)) END
    ELSE 'att' + CAST(dbo.[customentityattributes].attributeid AS nvarchar(10))
  END AS field,
  CASE dbo.[customEntityAttributes].fieldtype
    WHEN '17' THEN N'CL'
    ELSE dbo.getFieldType(dbo.[customEntityAttributes].fieldtype, dbo.[customEntityAttributes].format)
  END AS fieldtype,
  dbo.[customentityattributes].display_name AS description,
  dbo.[customentityattributes].display_name AS comment,
  CAST(0 AS bit) AS normalview,
  dbo.[customentityattributes].is_key_field AS idfield,
  dbo.[customentities].tableid AS viewgroupid,
  CAST(0 AS bit) AS genlist,
  CAST(50 AS int) AS width,
  CAST(0 AS bit) AS cantotal,
  CAST(0 AS bit) AS printout,
  CASE dbo.[customentityattributes].fieldtype
    WHEN 4 THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
  END AS valuelist,
  CAST(1 AS bit) AS allowimport,
  dbo.[customentityattributes].mandatory,
  dbo.[customentityattributes].modifiedon AS amendedon,
  dbo.[customentityattributes].relatedtable AS lookuptable,
  dbo.[tables].keyfield AS lookupfield,
  CAST(0 AS bit) AS useforlookup,
  CAST(1 AS bit) AS workflowUpdate,
  CAST(1 AS bit) AS workflowSearch,
  dbo.[customentityattributes].maxlength AS length,
  CAST(0 AS bit) AS Expr1,
  NULL AS Expr2,
  CAST(1 AS int) AS fieldFrom,
  CAST(0 AS bit) AS allowDuplicateChecking,
  NULL AS classPropertyName,
  CAST(0 AS tinyint) AS FieldCategory,
  customEntityAttributes.relatedtable AS RelatedTable,
  CASE (dbo.[customentityattributes].fieldtype)
    WHEN '22' THEN CAST(1 AS bit)
    WHEN '9' THEN CASE dbo.[customentityattributes].relationshiptype
        WHEN 1 THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
      END
	ELSE CAST(0 AS bit)
  END AS IsForeignKey,
  NULL AS associatedFieldForDuplicateChecking,
  NULL AS DuplicateCheckingSource,
  NULL AS DuplicateCheckingCalculation,
  [Encrypted]
FROM dbo.[customentityattributes]
INNER JOIN dbo.[customentities]
  ON dbo.[customentities].entityid = dbo.[customentityattributes].entityid
LEFT JOIN tables
  ON tables.tableid = dbo.[customentityattributes].relatedtable
UNION
SELECT
  fieldid,
  tableid,
  field,
  fieldtype,
  description,
  description AS comment,
  CAST(0 AS bit) AS normalview,
  idfield,
  NULL AS viewgroupid,
  CAST(0 AS bit) AS genlist,
  CAST(50 AS int) AS width,
  CAST(0 AS bit) AS cantotal,
  CAST(0 AS bit) AS Expr1,
  CAST(0 AS bit) AS Expr2,
  CAST(0 AS bit) AS Expr3,
  CAST(0 AS bit) AS Expr4,
  NULL AS amendedon,
  NULL AS lookuptable,
  NULL AS lookupfield,
  CAST(0 AS bit) AS useforlookup,
  CAST(1 AS bit) AS workflowUpdate,
  CAST(1 AS bit) AS workflowSearch,
  0 AS length,
  CAST(0 AS bit) AS Expr5,
  NULL AS Expr6,
  CAST(1 AS int) AS fieldFrom,
  CAST(0 AS bit) AS allowDuplicateChecking,
  NULL AS classPropertyName,
  CAST(0 AS tinyint) AS FieldCategory,
  NULL AS RelatedTable,
  CAST(0 AS bit) AS IsForeignKey,
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
  CAST(1 AS int) AS fieldFrom,
  CAST(0 AS bit) AS allowDuplicateChecking,
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
  'udf' + CAST(userdefineid AS nvarchar(10)) AS field,
  dbo.getFieldType(fieldtype, format) AS fieldtype,
  display_name AS description,
  display_name AS comment,
  CAST(1 AS bit) AS normalview,
  CAST(0 AS bit) AS idfield,
  dbo.getUserdefinedViewGroup(tableid, fieldtype) AS viewgroupid,
  CAST(0 AS bit) AS genlist,
  CAST(50 AS int) AS width,
  CAST(0 AS bit) AS cantotal,
  CAST(0 AS bit) AS printout,
  CASE userdefined.fieldtype
    WHEN 4 THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
  END AS valuelist,
  CAST(1 AS bit) AS allowimport,
  mandatory,
  ModifiedOn AS amendedon,
  NULL AS lookuptable,
  NULL AS lookupfield,
  CAST(0 AS bit) AS useforlookup,
  CAST(1 AS bit) AS workflowUpdate,
  CAST(1 AS bit) AS workflowSearch,
  maxlength AS length,
  CAST(0 AS bit) AS Expr1,
  NULL AS Expr2,
  CAST(2 AS int) AS fieldFrom,
  CAST(0 AS bit) AS allowDuplicateChecking,
  NULL AS classPropertyName,
  CAST(0 AS tinyint) AS FieldCategory,
  relatedTable AS RelatedTable,
  CASE fieldtype
    WHEN 9 THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
  END AS IsForeignKey,
  NULL AS associatedFieldForDuplicateChecking,
  NULL AS DuplicateCheckingSource,
  NULL AS DuplicateCheckingCalculation,
  CAST(0 AS BIT) AS [Encrypted]
FROM dbo.userdefined

GO