


CREATE PROCEDURE [dbo].[saveEmployeeWorkLocation]
@employeelocationid int,
@employeeid int,
@locationid int,
@startdate DateTime,
@enddate DateTime,
@active bit,
@temporary bit,
@date DateTime,
@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS

DECLARE @count INT;
declare @title1 nvarchar(500);
select @title1 = username from employees where employeeid = @employeeid;

declare @recordTitle nvarchar(2000);
set @recordTitle = (select + 'Work location for ' + @title1);

if (@employeelocationid = 0)
BEGIN
	insert into employeeWorkLocations (employeeid, locationid, startdate, enddate, active, [temporary], createdon, createdby) values (@employeeid, @locationid, @startdate, @enddate, @active, @temporary, @date, @userid);
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
	declare @oldactive bit;
	declare @oldtemporary bit;
	select @oldlocationid = locationid, @oldstartdate = startdate, @oldenddate = enddate, @oldactive = active, @oldtemporary = [temporary] from employeeWorkLocations where employeelocationid = @employeelocationid;

	update employeeWorkLocations set locationid = @locationid, startdate = @startdate, enddate = @enddate, active = @active, [temporary] = @temporary, modifiedby = @userid, modifiedon = @date where employeelocationid = @employeelocationid;
	
	if @CUemployeeID > 0
	Begin
		if @oldlocationid <> @locationid
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeelocationid, '12bba0c5-29f8-45aa-8795-869b5dff65da', @oldlocationid, @locationid, @recordtitle, null;
		if @oldstartdate <> @startdate
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeelocationid, '9c7aec4a-518e-42f6-a9cd-aa54e80af57e', @oldstartdate, @startdate, @recordtitle, null;
		if @oldenddate <> @enddate
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeelocationid, 'a3a80342-5905-4376-982b-6975ba8d3f06', @oldenddate, @enddate, @recordtitle, null;
		if @oldactive <> @active
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeelocationid, 'dcb8c037-c60d-40fb-97e1-1cef8d467492', @oldactive, @active, @recordtitle, null;
		if @oldtemporary <> @temporary
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeelocationid, '94152ff6-e3c4-49e1-abf9-12a144573eb6', @oldtemporary, @temporary, @recordtitle, null;
	end
end

Update employees set modifiedon=getdate() WHERE employeeid=@employeeid;
		
return @employeelocationid




 
