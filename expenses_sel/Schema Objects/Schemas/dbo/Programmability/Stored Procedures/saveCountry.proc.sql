
CREATE PROCEDURE [dbo].[saveCountry]
@subAccountId int,
@countryid int,
@globalcountryid int,
@date DateTime,
@userid INT,
@CUemployeeID INT,
@CUdelegateID INT
AS
DECLARE @count INT;
DECLARE @country nvarchar(50);

if @countryid = 0
BEGIN
	IF @subAccountId IS NOT NULL
	BEGIN
		SET @count = (SELECT COUNT(*) FROM countries WHERE globalcountryid = @globalcountryid AND subAccountId = @subAccountId);
	END
	ELSE
	BEGIN
		SET @count = (SELECT COUNT(*) FROM countries WHERE globalcountryid = @globalcountryid);
	END
	
	IF @count > 0
		RETURN -1;

	INSERT INTO countries (subAccountId, globalcountryid, createdon, createdby) values (@subAccountId, @globalcountryid, @date, @userid);
	set @countryid = scope_identity()
	SET @country = (SELECT country FROM global_countries WHERE globalcountryid = @globalcountryid);
	
	if @CUemployeeID > 0
	BEGIN
		EXEC addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 19, @countryid, @country, @subAccountId;
	END

	return @countryid
END
