Create FUNCTION [dbo].[getCurrencyLabel] (@param int)  
RETURNS varchar (200) AS  
BEGIN 
Declare @currencyLabel varchar(200)
set @currencyLabel = (select label from dbo.global_currencies where globalcurrencyid = (select globalcurrencyid from currencies where currencyid = @param))

return @currencyLabel
END