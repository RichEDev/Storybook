


CREATE PROCEDURE [dbo].[saveEmployeeHomeLocation]
@employeelocationid int,
@employeeid int,
@locationid int,
@startdate DateTime,
@enddate DateTime,
@date DateTime,
@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS

DECLARE @count INT;
declare @title1 nvarchar(500);
select @title1 = username from employees where employeeid = @employeeid;
declare @recordTitle nvarchar(2000);
set @recordTitle = (select + 'Home location for ' + @title1);

if (@employeelocationid = 0)
BEGIN

	insert into employeeHomeLocations (employeeid, locationid, startdate, enddate, createdon, createdby) values (@employeeid, @locationid, @startdate, @enddate, @date, @userid);
	set @employeelocationid = scope_identity();
	
	if @CUemployeeID > 0
	Begin
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeelocationid, @recordTitle, null;
	end
end
else
BEGIN
	
	declare @oldlocationid int;
	declare @oldstartdate DateTime;
	declare @oldenddate DateTime;
	select @oldlocationid = locationid, @oldstartdate = startdate, @oldenddate = enddate from employeeHomeLocations where employeelocationid = @employeelocationid;

	update employeeHomeLocations set locationid = @locationid, startdate = @startdate, enddate = @enddate, modifiedby = @userid, modifiedon = @date where employeelocationid = @employeelocationid
		
	if @CUemployeeID > 0
	Begin
		if @oldlocationid <> @locationid
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeelocationid, 'c3ea2dc0-3971-4e01-b6b9-30727c96bc67', @oldlocationid, @locationid, @recordtitle, null;
		if @oldstartdate <> @startdate
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeelocationid, '7969db25-d2c4-478b-92a2-5b08bdec1326', @oldstartdate, @startdate, @recordtitle, null;
		if @oldenddate <> @enddate
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeelocationid, 'fea8d6a3-3daa-4d65-bc6d-9f8f47647e81', @oldenddate, @enddate, @recordtitle, null;
	end
end


		Update employees set modifiedon=getdate() WHERE employeeid=@employeeid;
return @employeelocationid





 
