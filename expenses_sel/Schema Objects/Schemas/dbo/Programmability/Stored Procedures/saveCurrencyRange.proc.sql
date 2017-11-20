



CREATE PROCEDURE [dbo].[saveCurrencyRange]

@currencyid int,
@currencyrangeid int,
@startdate DateTime,
@enddate DateTime,
@date DateTime,
@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS


declare @recordTitle nvarchar(2000);
select @recordTitle = label from global_currencies where globalcurrencyid = (select globalcurrencyid from currencies where currencyid = @currencyid);

if @currencyrangeid = 0
BEGIN
	INSERT INTO currencyranges (currencyid, startdate, enddate, createdon, createdby) VALUES (@currencyid, @startdate, @enddate, @date, @userid);
	set @currencyrangeid = scope_identity();

	if @CUemployeeID > 0
	BEGIN
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencyrangeid, @recordTitle, null;
	END
END
else
BEGIN
	declare @oldstartdate DateTime;
	declare @oldenddate DateTime;
	select @oldstartdate = startdate, @oldenddate = enddate from currencyranges WHERE currencyrangeid = @currencyrangeid;

	UPDATE currencyranges SET startdate = @startdate, enddate = @enddate, modifiedon = @date, modifiedby = @userid WHERE currencyrangeid = @currencyrangeid;

	if @CUemployeeID > 0
	BEGIN
		if @oldstartdate <> @startdate
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencyrangeid, 'fbfded6e-6e0c-4a12-8b7a-a89e8a1d64b6', @oldstartdate, @startdate, @recordtitle, null;
		if @oldenddate <> @enddate
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 20, @currencyrangeid, 'ad8ff643-a693-4fdd-a643-a2aa0be058ca', @oldenddate, @enddate, @recordtitle, null;
	END
END

return @currencyrangeid




 
