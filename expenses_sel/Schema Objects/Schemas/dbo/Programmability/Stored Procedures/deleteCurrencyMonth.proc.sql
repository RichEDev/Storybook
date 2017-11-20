


CREATE PROCEDURE [dbo].[deleteCurrencyMonth]
	@currencymonthid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	declare @recordTitle nvarchar(2000);
	select @recordTitle = label from global_currencies where globalcurrencyid = (select globalcurrencyid from currencies where currencyid = (select currencyid from currencymonths where currencymonthid = @currencymonthid));

	DELETE FROM currencymonths WHERE currencymonthid = @currencymonthid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencymonthid, @recordTitle, null;
END
