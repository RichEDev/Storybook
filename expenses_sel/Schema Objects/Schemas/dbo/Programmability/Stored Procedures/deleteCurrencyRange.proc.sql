


CREATE PROCEDURE [dbo].[deleteCurrencyRange]
	@currencyrangeid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	declare @recordTitle nvarchar(2000);
	select @recordTitle = label from global_currencies where globalcurrencyid = (select globalcurrencyid from currencies where currencyid = (select currencyid from currencyranges where currencyrangeid = @currencyrangeid));
	
	DELETE FROM range_exchangerates WHERE currencyrangeid = @currencyrangeid;
	DELETE FROM currencyranges WHERE currencyrangeid = @currencyrangeid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencyrangeid, @recordTitle, null;
END
