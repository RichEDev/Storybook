CREATE VIEW dbo.globalESRElementFields
AS
SELECT     globalESRElementFieldID, globalESRElementID, ESRElementFieldName, isMandatory, isControlColumn, isSummaryColumn, isRounded
FROM         [$(targetMetabase)].dbo.globalESRElementFields AS globalESRElementFields_1

