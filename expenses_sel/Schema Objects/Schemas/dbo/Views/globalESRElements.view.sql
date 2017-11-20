
CREATE VIEW [dbo].[globalESRElements]
AS
SELECT globalESRElementID, ESRElementName
FROM  [$(metabaseExpenses)].dbo.globalESRElements

