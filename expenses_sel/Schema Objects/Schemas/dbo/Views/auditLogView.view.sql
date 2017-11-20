

CREATE VIEW [dbo].[auditLogView]
AS
SELECT     dbo.auditLog.logid, dbo.auditLog.datestamp, auditlog.elementid, metabaseExpenses.dbo.elementsBase.elementFriendlyName, dbo.auditLog.employeeID, 
                      CASE WHEN employees.username IS NULL THEN auditlog.employeeUsername WHEN employees.username IS NOT NULL 
                      THEN employees.username END AS username, CASE WHEN delegateemployees.username IS NULL 
                      THEN auditlog.delegateUsername WHEN delegateemployees.username IS NOT NULL THEN delegateemployees.username END AS delegate, dbo.auditLog.action, 
                      dbo.fields.description, dbo.auditLog.oldvalue, dbo.auditLog.newvalue, dbo.auditLog.recordTitle
FROM         dbo.auditLog INNER JOIN
                      [$(metabaseExpenses)].dbo.elementsBase ON metabaseExpenses.dbo.elementsBase.elementID = dbo.auditLog.elementID LEFT OUTER JOIN
                      dbo.fields ON dbo.fields.fieldid = dbo.auditLog.field LEFT OUTER JOIN
                      dbo.employees ON dbo.employees.employeeid = dbo.auditLog.employeeID LEFT OUTER JOIN
                      dbo.employees AS delegateemployees ON delegateemployees.employeeid = dbo.auditLog.delegateID


