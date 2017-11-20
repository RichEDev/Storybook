CREATE PROCEDURE [dbo].GetPrimaryCountry 
 @employeeid Int  
AS
BEGIN
SELECT global_countries.country,countries.countryid 
FROM employees  
INNER  JOIN countries ON (employees.primarycountry = countries.countryid) 
INNER JOIN global_countries  ON (global_countries.globalcountryid = countries.globalcountryid) 
WHERE employees.employeeid = @employeeid
END
