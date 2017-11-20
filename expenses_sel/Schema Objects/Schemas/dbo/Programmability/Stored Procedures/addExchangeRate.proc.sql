
CREATE PROCEDURE [dbo].[addExchangeRate]
	@tocurrencyid int,
	@id int,
	@tableType tinyint,
	@exchangerate float,
	@date DateTime,
	@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS

declare @recordTitle nvarchar(2000);
declare @title1 nvarchar(500);
declare @title2 nvarchar(500);
select @title1 = label from global_currencies where globalcurrencyid = (select globalcurrencyid from currencies where currencyid = @id);
select @title2 = label from global_currencies where globalcurrencyid = (select globalcurrencyid from currencies where currencyid = @tocurrencyid);

if @tableType = 1
	Begin
		insert into static_exchangerates (currencyid, tocurrencyid, exchangerate, createdon, createdby)
		values (@id, @tocurrencyid, @exchangerate, @date, @userid);

		set @recordTitle = (select 'Static exchange rate ' + @title1 + ' to ' + @title2);
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @id, @recordTitle, null;
	end
if @tableType = 2
	Begin
		insert into monthly_exchangerates (currencymonthid, tocurrencyid, exchangerate)
        values (@id, @tocurrencyid, @exchangerate);

		set @recordTitle = (select 'Monthly exchange rate ' + @title1 + ' to ' + @title2);
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @id, @recordTitle, null;
	end
if @tableType = 3
	Begin
		insert into range_exchangerates (currencyrangeid, tocurrencyid, exchangerate)
        values (@id, @tocurrencyid, @exchangerate);

		set @recordTitle = (select 'Date Range exchange rate ' + @title1 + ' to ' + @title2);
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @id, @recordTitle, null;
	End
