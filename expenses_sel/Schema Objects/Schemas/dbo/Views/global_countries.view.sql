

CREATE VIEW [dbo].[global_countries]
	AS
	SELECT     globalcountryid, country, countrycode, createdon, modifiedon, alpha3CountryCode, numeric3code
	FROM         [$(metabaseExpenses)].dbo.global_countries AS global_countries_1

