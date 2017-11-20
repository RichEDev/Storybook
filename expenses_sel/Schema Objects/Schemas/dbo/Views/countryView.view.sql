
CREATE VIEW dbo.countryView
	AS
	SELECT
	countries.countryid,
	global_countries.country,
	global_countries.countrycode,
	global_countries.alpha3CountryCode,
	countries.globalcountryid,
	countries.CreatedOn,
	dbo.getEmployeeFullName(countries.CreatedBy) AS createdBy,
	countries.ModifiedOn,
	dbo.getEmployeeFullName(countries.modifiedBy) AS modifiedBy,
	countries.archived
	FROM countries
	INNER JOIN global_countries ON global_countries.globalcountryid = countries.globalcountryid
