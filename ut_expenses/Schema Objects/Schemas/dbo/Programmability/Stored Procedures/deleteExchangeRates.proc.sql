CREATE PROCEDURE [dbo].[deleteExchangeRates]
	@id int,
	@tableType tinyint,
@CUemployeeID INT,
@CUdelegateID INT
AS
declare @recordTitle nvarchar(2000);
declare @subAccountId int;
declare @title1 nvarchar(500);
select @title1 = label from global_currencies where globalcurrencyid = (select globalcurrencyid from currencies where currencyid = @id);
select @subAccountId = subAccountId from currencies where currencyid = @id;

if @tableType = 1
	Begin
		DELETE FROM static_exchangerates WHERE currencyid = @id;

		set @recordTitle = (select 'Static exchange rates for ' + @title1);
		exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @id, @recordTitle, @subAccountId;
	end
if @tableType = 2
	Begin
		DELETE FROM monthly_exchangerates WHERE currencymonthid = @id;

		set @recordTitle = (select 'Monthly exchange rates for ' + @title1);
		exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @id, @recordTitle, @subAccountId;
	end
if @tableType = 3
	Begin
		DELETE FROM range_exchangerates where currencyrangeid = @id;

		set @recordTitle = (select 'Date Range exchange rates for ' + @title1);
		exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @id, @recordTitle, @subAccountId;
	End
