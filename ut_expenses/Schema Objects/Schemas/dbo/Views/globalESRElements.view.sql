CREATE VIEW dbo.globalESRElements
AS
SELECT     globalESRElementID, ESRElementName
FROM         [$(targetMetabase)].dbo.globalESRElements AS globalESRElements_1

