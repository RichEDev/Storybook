CREATE VIEW dbo.global_countries
AS
SELECT     globalcountryid, country, countrycode, createdon, modifiedon, alpha3CountryCode, numeric3Code, postcodeAnywhereEnabled
FROM         [$(targetMetabase)].dbo.global_countries AS global_countries_1

