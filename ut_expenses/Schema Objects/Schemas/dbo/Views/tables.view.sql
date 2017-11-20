CREATE VIEW [dbo].[tables]
AS
SELECT     tableid, tablename, jointype, allowreporton, description, primarykey, allowimport, keyfield, amendedon, allowworkflow, allowentityrelationship, hasUserDefinedFields, 
                      userdefined_table, NULL AS parentTableID, elementID, subAccountIDField, CAST(0 AS INT) tableFrom, CAST(0 AS BIT) AS isSystemView, relabel_param
FROM         [$(targetMetabase)].dbo.tables_base
UNION
SELECT     tableid, 'custom_' + masterTableName AS tablename, CAST(2 AS TINYINT) AS jointype,  CASE systemview WHEN 1 THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS allowreporton, plural_name,
                          (SELECT     fieldid
                            FROM          dbo.[customEntityAttributes]
                            WHERE      (is_key_field = 1) AND (entityid = dbo.[customEntities].entityid)
                            UNION ALL
                            SELECT     fieldid
                            FROM         dbo.[customFields]
                            WHERE     (idfield = 1) AND (tableid = dbo.[customEntities].tableid)) AS primarykey, CAST(1 AS BIT) AS allowimport,
                          (SELECT     fieldid
                            FROM          dbo.[customEntityAttributes]
                            WHERE      (is_audit_identity = 1) AND (entityid = dbo.[customEntities].entityid)) AS keyfield, modifiedon, CAST(1 AS BIT) AS allowworkflow, CAST(1 AS BIT) 
                      AS allowentityrelationship, CAST(0 AS BIT) AS hasuserdefinedfields, NULL AS userdefined_table, NULL AS parentTableID, 135 AS elementID, NULL 
                      AS subAccountIDField, CAST(1 AS INT) tableFrom, systemview AS isSystemView, NULL AS relabel_param
FROM         dbo.[customEntities]
UNION
SELECT     attachmentTableID, 'custom_' + masterTableName + '_attachments' AS tablename, CAST(1 AS TINYINT) AS jointype, CAST(0 AS BIT) AS allowreporton, 
                      plural_name + ' Attachments' AS Expr1,
                          (SELECT     fieldid
                            FROM          dbo.customEntityAttachmentFields
                            WHERE      (field = 'id') AND (tableid = custom_entities_1.attachmentTableID)) AS primarykey, CAST(1 AS BIT) AS allowimport, NULL AS keyfield, modifiedon, 
                      CAST(1 AS BIT) AS allowworkflow, CAST(0 AS BIT) AS allowentityrelationship, CAST(0 AS BIT) AS hasuserdefinedfields, NULL AS userdefined_table, NULL 
                      AS parentTableID, NULL AS elementID, NULL AS subAccountIDField, CAST(1 AS INT) tableFrom, CAST(0 AS BIT) AS isSystemView, NULL AS relabel_param
FROM         dbo.[customEntities] AS custom_entities_1
WHERE     (enableAttachments = 1 OR allowdocmergeaccess = 1)
UNION
SELECT     audienceTableID, 'custom_' + masterTableName + '_audiences' AS tablename, CAST(1 AS TINYINT) AS jointype, CAST(0 AS BIT) AS allowreporton, 
                      plural_name + ' Audiences' AS Expr1,
                          (SELECT     fieldid
                            FROM          dbo.[customFields] AS custom_fields_1
                            WHERE      (idfield = 1) AND (tableid = custom_entities_2.audienceTableID)) AS primarykey, CAST(1 AS BIT) AS allowimport, NULL AS keyfield, modifiedon, 
                      CAST(1 AS BIT) AS allowworkflow, CAST(0 AS BIT) AS allowentityrelationship, CAST(0 AS BIT) AS hasuserdefinedfields, NULL AS userdefined_table, NULL 
                      AS parentTableID, NULL AS elementID, NULL AS subAccountIDField, CAST(1 AS INT) tableFrom, CAST(0 AS BIT) AS isSystemView, NULL AS relabel_param
FROM         dbo.[customEntities] AS custom_entities_2
WHERE     (audienceViewType > 0)
UNION
SELECT     tableid, tablename, jointype, allowreporton, description, primarykey, allowimport, keyfield, amendedon, allowworkflow, allowentityrelationship, hasUserDefinedFields, 
                      userdefined_table, parentTableID, NULL AS elementID, NULL AS subAccountIDField, CAST(2 AS INT) tableFrom, CAST(0 AS BIT) AS isSystemView, NULL AS relabel_param
FROM         dbo.[customTables]
