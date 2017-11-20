CREATE PROCEDURE [dbo].[saveMobileItem] 
 @employeeid int,
 @otherdetails nvarchar(max),
 @reasonid int,
 @total money,
 @subcatid int,
 @date datetime,
 @currencyid int,
 @miles decimal(18,2),
 @quantity float,
 @fromLocation nvarchar(200),
 @toLocation nvarchar(200),
 @allowancestartdate datetime,
 @allowanceenddate datetime,
 @allowancetype int,
 @allowancedeductamount money,
 @itemNotes nvarchar(max), 
 @deviceTypeId int,
 @mileageJson nvarchar(max),
 @mobileDeviceID int,
 @mobileExpenseID int

AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 if @reasonid is not null and @reasonid > 0
 begin
  if not exists (select reasonid from reasons where reasonid = @reasonid)
  begin
   return -5; -- MobileReturnCode for ReasonIsInvalid
  end
 end
 
 if @currencyid is not null
 begin
  if not exists (select currencyid from currencies where archived = 0 and currencyid = @currencyid)
  begin
   return -6; -- MobileReturnCode for CurrencyIsInvalid
  end
 end
 
 if @allowancetype is not null
 begin 
  if not exists (select allowanceid from allowances where allowanceid = @allowancetype)
  begin
   return -16; -- MobileReturnCode for AllowanceIsInvalid
  end
 end
 
 if @subcatid is not null
 begin
	 if not exists
	 (
	  select item_roles.itemroleid from employee_roles
	  inner join item_roles on item_roles.itemroleid = employee_roles.itemroleid
	  inner join rolesubcats on rolesubcats.roleid = item_roles.itemroleid
	  where employee_roles.employeeid = @employeeid and rolesubcats.subcatid = @subcatid
	 )
	 begin
	  return -7; -- MobileReturnCode for SubcatIsInvalid
	 end
 end

 declare @identity int
 
 set @identity = (select mobileID from mobileExpenseItems where mobileDeviceID = @mobileDeviceID and mobileExpenseID = @mobileExpenseID)

 if @identity is not null
 begin
	return @identity -- If we've already got an expense item from the same device with the same mobileExpenseID then return the existing mobileID
 end
 
 -- Insert statements for procedure here

 insert into mobileExpenseItems (employeeid, otherdetails, reasonid, total, subcatid, date, currencyid, miles, quantity, fromLocation, toLocation, allowancestartdate, allowanceenddate, itemNotes, deviceTypeId, allowanceid, allowancededuct, mileageJourneySteps, mobileDeviceID, mobileExpenseID) values (@employeeID, @otherdetails, @reasonid, @total, @subcatid, @date, @currencyid, @miles, @quantity, @fromLocation, @toLocation, @allowancestartdate, @allowanceenddate, @itemNotes, @deviceTypeId, @allowancetype, @allowancedeductamount, @mileageJson, @mobileDeviceID, @mobileExpenseID);
 
 set @identity = scope_identity();
 return @identity
END
GO