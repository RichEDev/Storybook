CREATE PROCEDURE [dbo].[deleteSupplierAddress] (@addressid int,
@CUemployeeID INT,
@CUdelegateID INT)
AS
begin
	declare @title1 nvarchar(500);
	declare @subAccountId int;
	
	set @subAccountId = (select subAccountId from supplier_addresses inner join supplier_details on supplier_addresses.supplierid = supplier_details.supplierid where addressid = @addressid)
	select @title1 = address_title from supplier_addresses where addressid = @addressid;

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Supplier Address - ' + @title1);

	update supplier_details set primary_addressid = null where primary_addressid = @addressid;
	update supplier_contacts set business_addressid = null where business_addressid = @addressid;
	update supplier_contacts set home_addressid = null where home_addressid = @addressid;

	delete from dbo.supplier_addresses where addressid = @addressid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, @recordTitle, @subAccountId;
end
