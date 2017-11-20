CREATE PROCEDURE dbo.deleteAddress (@addressid int)
AS
begin
	update supplier_details set primary_addressid = null where primary_addressid = @addressid
	update supplier_contacts set business_addressid = null where business_addressid = @addressid
	update supplier_contacts set home_addressid = null where home_addressid = @addressid

	delete from dbo.supplier_addresses where addressid = @addressid
end
