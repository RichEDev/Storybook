CREATE PROCEDURE [dbo].[changeCurrencyStatus]
@currencyid int,
@archive bit,
@CUemployeeID INT,
@CUdelegateID INT
AS
DECLARE @tmpCount int;
DECLARE @claimCount int;

IF @archive = 1
BEGIN
	SET @tmpCount = (SELECT count(stringValue) FROM accountProperties WHERE stringKey = 'baseCurrency' and stringValue = @currencyid)
	IF @tmpCount > 0
		RETURN 1;	
	SET @tmpCount = (SELECT count(contractCurrency) FROM contract_details WHERE contractCurrency = @currencyid)
	IF @tmpCount > 0
		RETURN 2;
	SET @tmpCount = (SELECT count(supplier_currency) FROM supplier_details WHERE supplier_currency = @currencyid)
	IF @tmpCount > 0
		RETURN 3;
	SET @tmpCount = (SELECT count(currencyId) FROM contract_productdetails WHERE currencyId = @currencyid)
	IF @tmpCount > 0
		RETURN 4;
	SET @tmpCount = (select count(CurrencyId) from bankaccounts where [CurrencyId] = @currencyid);
	IF @tmpCount > 0
		RETURN 9;
END

declare @oldarchive bit;
declare @recordtitle nvarchar(2000);
declare @subAccountId int;
select @recordtitle = label from global_currencies where globalcurrencyid = (select globalcurrencyid from currencies where currencyid = @currencyid);

UPDATE currencies SET archived = @archive, @subAccountId = subAccountId WHERE currencyid = @currencyid;

if @oldarchive <> @archive
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencyid, '5fed50d9-f5c7-497f-9ec3-827c9035261d', @oldarchive, @archive, @recordtitle, @subAccountId;
