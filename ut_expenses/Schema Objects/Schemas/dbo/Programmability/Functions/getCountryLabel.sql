/* Get global country label using countryid*/

CREATE FUNCTION [dbo].[getCountryLabel] (@param int)  
RETURNS nvarchar (100) AS  
BEGIN 
Declare @countryLabel nvarchar(100)
set @countryLabel = (select country from dbo.global_countries where globalcountryid = (select globalcountryid from countries where countryid = @param))

return @countryLabel
END