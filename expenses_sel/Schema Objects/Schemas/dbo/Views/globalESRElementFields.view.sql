
CREATE VIEW [dbo].[globalESRElementFields]
AS
SELECT globalESRElementFieldID, globalESRElementID, ESRElementFieldName, isMandatory, isControlColumn
FROM  [$(metabaseExpenses)].dbo.globalESRElementFields AS globalESRElementFields_1

