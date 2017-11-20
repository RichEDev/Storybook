
CREATE PROCEDURE [dbo].[saveCurrency]
@subAccountId int,
@currencyid int,
@globalcurrencyid int,
@positiveFormat tinyint,
@negativeFormat tinyint,
@date DateTime,
@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
DECLARE @currency nvarchar(50);
DECLARE @count INT;

if @currencyid = 0
BEGIN
	IF @subAccountId IS NOT NULL
	BEGIN
		SET @count = (select count(*) from currencies where globalcurrencyid = @globalcurrencyid and subAccountId = @subAccountId);	
	END
	ELSE
	BEGIN
		SET @count = (select count(*) from currencies where globalcurrencyid = @globalcurrencyid);	
	END
	
	IF @count > 0
	BEGIN
		RETURN -1;
	END
	
	INSERT INTO currencies (subAccountId, globalcurrencyid, positiveFormat, negativeFormat, createdon, createdby) values (@subAccountId, @globalcurrencyid, @positiveFormat, @negativeFormat, @date, @userid);
	set @currencyid = scope_identity();
	SET @currency = (SELECT label FROM global_currencies WHERE globalcurrencyid = @globalcurrencyid);
	
	if @CUemployeeID > 0
	BEGIN
		EXEC addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencyid, @currency, @subAccountId;
	END
END
else
BEGIN
	declare @oldpositiveFormat tinyint;
	declare @oldnegativeFormat tinyint;
	select @oldpositiveFormat = positiveFormat, @oldnegativeFormat = negativeFormat from currencies WHERE currencyid = @currencyid;

	UPDATE currencies SET positiveFormat = @positiveFormat, negativeFormat = @negativeFormat, modifiedon = @date, modifiedby = @userid WHERE currencyid = @currencyid;

	SET @currency = (SELECT label FROM global_currencies WHERE globalcurrencyid = @globalcurrencyid);
	
	if @CUemployeeID > 0
	BEGIN
		if @oldpositiveFormat <> @positiveFormat
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencyid, '8389db8b-3120-4526-a898-86a7c6140ab7', @oldpositiveFormat, @positiveFormat, @currency, @subAccountId;
		if @oldnegativeFormat <> @negativeFormat
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencyid, '6228d095-58ee-4599-b862-c21f5c0b5755', @oldnegativeFormat, @negativeFormat, @currency, @subAccountId;
	END
END

return @currencyid
