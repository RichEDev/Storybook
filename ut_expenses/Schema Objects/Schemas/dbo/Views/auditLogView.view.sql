CREATE VIEW dbo.auditLogView
AS
SELECT     dbo.auditLog.logid, dbo.auditLog.datestamp, dbo.auditLog.elementID, [$(targetMetabase)].dbo.elementsBase.elementFriendlyName, 
                      dbo.auditLog.employeeID,
					  CASE WHEN auditlog.employeeUsername IS NOT NULL THEN
					  (CASE WHEN employees.username IS NULL THEN auditlog.employeeUsername WHEN employees.username IS NOT NULL THEN employees.username END)
					  ELSE 'System' END AS username,
					  CASE WHEN delegateemployees.username IS NULL THEN auditlog.delegateUsername WHEN delegateemployees.username IS NOT NULL THEN delegateemployees.username END AS delegate,
					  dbo.auditLog.action, ISNULL(fields.description, Customerfields.description) as description , dbo.auditLog.oldvalue, dbo.auditLog.newvalue, dbo.auditLog.recordTitle
FROM         dbo.auditLog INNER JOIN
                      [$(targetMetabase)].dbo.elementsBase ON [$(targetMetabase)].dbo.elementsBase.elementID = dbo.auditLog.elementID 
					  LEFT OUTER JOIN [$(targetMetabase)].dbo.fields_base as fields ON fields.fieldid = dbo.auditLog.field 
					  LEFT OUTER JOIN dbo.Customerfields as Customerfields ON Customerfields.fieldid = dbo.auditLog.field 
					  LEFT OUTER JOIN dbo.employees ON dbo.employees.employeeid = dbo.auditLog.employeeID 
					  LEFT OUTER JOIN dbo.employees AS delegateemployees ON delegateemployees.employeeid = dbo.auditLog.delegateID
