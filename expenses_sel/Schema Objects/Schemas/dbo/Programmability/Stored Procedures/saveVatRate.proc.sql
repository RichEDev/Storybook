
CREATE PROCEDURE [dbo].[saveVatRate] 
	@countrysubcatid int,
	@countryid int,
	@subcatid int,
	@vat float,
	@vatpercent float,
	@date DateTime,
	@userid INT,
	@CUemployeeID INT,
	@CUdelegateID INT	
AS

declare @recordTitle nvarchar(2000);
select @recordTitle = country from global_countries where globalcountryid = (select globalcountryid from countries where countryid = @countryid);

IF @countrysubcatid = 0
BEGIN
	INSERT INTO countrysubcats (countryid, subcatid, vat, vatpercent, createdon, createdby) VALUES (@countryid, @subcatid, @vat, @vatpercent, @date, @userid);
	SET @countrysubcatid = scope_identity();

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 19, @countrysubcatid, @recordTitle, null;
END
ELSE
BEGIN
	DECLARE @oldvat float;
	DECLARE @oldvatpercent float;
	SELECT @oldvat = vat, @oldvatpercent = vatpercent FROM countrysubcats WHERE countrysubcatid = @countrysubcatid;

	UPDATE countrysubcats SET vat = @vat, vatpercent = @vatpercent WHERE countrysubcatid = @countrysubcatid;

	IF @oldvat <> @vat
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 19, @countrysubcatid, 'e86f857c-68ec-4410-8e2a-3a1490d06e1d', @oldvat, @vat, @recordtitle, null;
	IF @oldvatpercent <> @vatpercent
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 19, @countrysubcatid, '6114b148-1cfa-43c0-b953-c6f1275fdbf3', @oldvatpercent, @vatpercent, @recordtitle, null;
END

RETURN @countrysubcatid
