CREATE PROCEDURE [dbo].[deleteCountry]
@countryid int,
@globalcountryid int,
@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS

DECLARE @tmpCount int;
DECLARE @country nvarchar(50);
DECLARE @subAccountId int;
BEGIN

	SET @tmpCount = (select count(expenseid) from savedexpenses where countryid = @countryid)
	IF @tmpCount > 0
		RETURN 1;
	SET @tmpCount = (select count(employeeid) from employees where primarycountry = @countryid)
	IF @tmpCount > 0
		RETURN 2;
	SET @tmpCount = (select count(addressid) from supplier_addresses where countryid = @countryid);
	if @tmpCount > 0
		RETURN 3;
	SET @subAccountId = (select subAccountId from countries where countryid = @countryid);
	
	delete from countries where countryid = @countryid;

	SET @country = (SELECT country FROM global_countries WHERE globalcountryid = @globalcountryid);
	EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 19, @countryid, @country, @subAccountId;

	RETURN 0;
END
