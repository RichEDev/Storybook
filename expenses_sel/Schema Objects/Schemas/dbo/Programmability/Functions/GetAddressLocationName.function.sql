

CREATE FUNCTION [dbo].[GetAddressLocationName] (@ID INT)

RETURNS nvarchar(250) AS  

BEGIN
	DECLARE @name nvarchar(50)
	SET @name = (SELECT company FROM companies WHERE companyid = @ID)
	RETURN @name
END

