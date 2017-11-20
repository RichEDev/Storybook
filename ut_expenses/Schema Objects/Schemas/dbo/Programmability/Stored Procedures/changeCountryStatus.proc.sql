CREATE PROCEDURE [dbo].[changeCountryStatus] 
@countryid int,
@archive bit,
@CUemployeeID INT,
@CUdelegateID INT
AS
DECLARE @tmpCount int;

IF @archive = 1
BEGIN
	SET @tmpCount = (select count(*) from employees where primarycountry = @countryid)
	IF @tmpCount > 0
		RETURN 1;
	SET @tmpCount = (select count(*) from savedexpenses where countryid = @countryid)
	IF @tmpCount > 0
		RETURN 2;
	SET @tmpCount = (select count(addressid) from supplier_addresses where countryid = @countryid);
	if @tmpCount > 0
		RETURN 3;
END

declare @recordtitle nvarchar(2000);
declare @subAccountId int;
select @recordtitle = country from global_countries where globalcountryid = (select globalcountryid from countries where countryid = @countryid);
declare @oldarchive bit;
select @oldarchive = archived, @subAccountId = subAccountId from countries where countryid = @countryid;

UPDATE countries SET archived = @archive, modifiedon = getdate(), modifiedby = @CUemployeeID WHERE countryid = @countryid;

if @oldarchive <> @archive
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 19, @countryid, 'fdfa66cc-5daf-411b-8290-a35470a08cd7', @oldarchive, @archive, @recordtitle, @subAccountId;

RETURN 0;
