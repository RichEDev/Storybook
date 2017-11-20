



CREATE PROCEDURE [dbo].[saveCurrencyMonth]

@currencyid int,
@currencymonthid int,
@year smallint,
@month tinyint,
@date DateTime,
@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
declare @recordTitle nvarchar(2000);
select @recordTitle = label from global_currencies where globalcurrencyid = (select globalcurrencyid from currencies where currencyid = @currencyid);

if @currencymonthid = 0
BEGIN
	INSERT INTO currencymonths (currencyid, [year], [month], createdon, createdby) VALUES (@currencyid, @year, @month, @date, @userid);
	set @currencymonthid = scope_identity();
	
	if @CUemployeeID > 0
	BEGIN
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencymonthid, @recordTitle, null;
	END
END
else
BEGIN
	declare @oldyear smallint;
	declare @oldmonth tinyint;
	select @oldyear = [year], @oldmonth = [month] from currencymonths WHERE currencymonthid = @currencymonthid;

	UPDATE currencymonths SET [year] = @year, [month] = @month, modifiedon = @date, modifiedby = @userid WHERE currencymonthid = @currencymonthid;
	
	if @CUemployeeID > 0
	BEGIN
		if @oldyear <> @year
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencymonthid, '9c38f8d2-d6ce-48cc-955c-1609106e43e0', @oldyear, @year, @recordtitle, null;
		if @oldmonth <> @month
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencymonthid, '3a1a189d-9763-45ee-8b63-a53704ea5a05', @oldmonth, @month, @recordtitle, null;
	END
END

return @currencymonthid



 
