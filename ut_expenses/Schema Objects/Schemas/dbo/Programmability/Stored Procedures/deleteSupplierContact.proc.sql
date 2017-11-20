CREATE PROCEDURE [dbo].[deleteSupplierContact] (@contactid int,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	declare @business_addressid int
	declare @private_addressid int
	declare @contact_addresscount int
	declare @supplier_addresscount int
	declare @subAccountId int;
	
	declare @recordTitle nvarchar(2000);
	select @recordTitle = contactname from supplier_contacts where contactid = @contactid;

	-- remove address if it not used anywhere else
	set @business_addressid = (SELECT business_addressid from supplier_contacts where contactid = @contactid)
	set @private_addressid = (SELECT home_addressid from supplier_contacts where contactid = @contactid)
	set @contact_addresscount = (SELECT COUNT(*) FROM supplier_contacts where (business_addressid = @business_addressid or home_addressid = @business_addressid) and contactid <> @contactid)
	set @supplier_addresscount = (SELECT COUNT(*) FROM supplier_details where primary_addressid = @business_addressid)
	set @subAccountId = (select subAccountId from supplier_contacts inner join supplier_details on supplier_contacts.supplierid = supplier_details.supplierid where supplier_contacts.contactid = @contactid);
	
	if @contact_addresscount = 0 and @supplier_addresscount = 0
	begin
		update supplier_contacts set home_addressid = null where home_addressid = @business_addressid
		update supplier_contacts set business_addressid = null where business_addressid = @business_addressid
		delete from supplier_addresses where addressid = @business_addressid
	end 

	set @contact_addresscount = (SELECT COUNT(*) FROM supplier_contacts where (business_addressid = @private_addressid or home_addressid = @private_addressid) and contactid <> @contactid)
	set @supplier_addresscount = (SELECT COUNT(*) FROM supplier_details where primary_addressid = @private_addressid)

	if @contact_addresscount = 0 and @supplier_addresscount = 0
	begin
		update supplier_contacts set home_addressid = null where home_addressid = @private_addressid
		update supplier_contacts set business_addressid = null where business_addressid = @private_addressid
		delete from supplier_addresses where addressid = @private_addressid
	end 

	delete from supplier_contacts where contactid = @contactid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @contactid, @recordTitle, @subAccountId;
END
