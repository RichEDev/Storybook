CREATE PROCEDURE [dbo].[deleteSupplier] (@supplierid int,
@CUemployeeID INT,
@CUdelegateID INT)
AS
begin
	declare @recordTitle nvarchar(2000);
	declare @subAccountId int;
	declare @primaryAddressId int;
	declare @tmpCount int;
	
	set @tmpCount = (select count(contractid) from contract_details where supplierId = @supplierid);
	if @tmpCount > 0
		return 1;
	-- don't permit delete if not started or in progress tasks exist
	set @tmpCount = (select count(taskid) from tasks where regardingId = @supplierid and regardingArea = 4 and (statusId = 0 or statusId = 1 or statusId = 2));
	if @tmpCount > 0
		return 2;
		
	select @recordTitle = suppliername, @subAccountId = subaccountid, @primaryAddressId = primary_addressid from supplier_details where supplierid = @supplierid;

	update supplier_details set primary_addressid = null where supplierid = @supplierid;
	delete from supplier_contacts where supplierid = @supplierid;
	delete from supplier_addresses where addressid = @primaryAddressId;
	delete from supplier_addresses where supplierid = @supplierid;
	delete from product_suppliers where supplierId = @supplierid;
	delete from tasks where regardingId = @supplierid and regardingArea = 4;
	delete from supplierNotes where supplierid = @supplierid;
	delete from supplier_details where supplierid = @supplierid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, @recordTitle, @subAccountId;
	
	return 0;
end

