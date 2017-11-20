CREATE VIEW locales
AS
SELECT localeID, localeName, localeCode, active FROM [$(targetMetabase)].dbo.locales
GO