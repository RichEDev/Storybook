CREATE PROCEDURE [dbo].[saveSupplierAddress] (
@userid int,
@addressid int,
@supplierid int,
@address_title nvarchar(250),
@addr_line1 nvarchar(150),
@addr_line2 nvarchar(150),
@town nvarchar(150),
@county nvarchar(150),
@postcode nvarchar(10),
@countryid int,
@switchboard nvarchar(30),
@fax nvarchar(30),
@private_address bit,
@CUemployeeID INT,
@CUdelegateID INT
)
AS
begin
declare @id int;
declare @count int

declare @recordTitle nvarchar(2000);
declare @subAccountId int;
set @subAccountId = (select subAccountId from supplier_details where supplierId = @supplierid);
set @recordTitle = (select 'Supplier Address - ' + @address_title);

IF @addressid = 0
begin

	insert into supplier_addresses (address_title,supplierid,addr_line1,addr_line2,town,county,postcode,countryid,switchboard,fax,private_address,createdon,createdby,modifiedon,modifiedby)
	values (@address_title,@supplierid,@addr_line1,@addr_line2,@town,@county,@postcode,@countryid,@switchboard,@fax,@private_address,getdate(),@userid,getdate(),@userid)

	set @id = scope_identity();

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @id, @recordTitle, @subAccountId;
end
else
begin
	declare @oldsupplierid int;
	declare @oldaddress_title nvarchar(250);
	declare @oldaddr_line1 nvarchar(150);
	declare @oldaddr_line2 nvarchar(150);
	declare @oldtown nvarchar(150);
	declare @oldcounty nvarchar(150);
	declare @oldpostcode nvarchar(10);
	declare @oldcountryid int;
	declare @oldswitchboard nvarchar(30);
	declare @oldfax nvarchar(30);
	declare @oldprivate_address bit;
	select @oldsupplierid = supplierid,
		@oldaddress_title = address_title,
		@oldaddr_line1 = addr_line1,
		@oldaddr_line2 = addr_line2,
		@oldtown = town,
		@oldcounty = county,
		@oldpostcode = postcode,
		@oldcountryid = countryid,
		@oldswitchboard = switchboard,
		@oldfax = fax,
		@oldprivate_address = private_address
		from supplier_addresses where addressid = @addressid;

	update supplier_addresses
	set
	supplierid = @supplierid,
	address_title = @address_title,
	addr_line1 = @addr_line1,
	addr_line2 = @addr_line2,
	town = @town,
	county = @county,
	postcode = @postcode,
	countryid = @countryid,
	switchboard = @switchboard,
	fax = @fax,
	private_address = @private_address,
	modifiedon = getdate(),
	modifiedby = @userid
	where addressid = @addressid

	set @id = @addressid;

	if @oldsupplierid <> @supplierid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, 'e2e87a10-56e1-4106-a707-739f228862a2', @oldsupplierid, @supplierid, @recordtitle, @subAccountId;
	if @oldaddress_title <> @address_title
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, '56772149-e24e-445d-ad21-948d051612f5', @oldaddress_title, @address_title, @recordtitle, @subAccountId;
	if @oldaddr_line1 <> @addr_line1
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, '4980a2f7-da59-44c9-9407-64da98b769d7', @oldaddr_line1, @addr_line1, @recordtitle, @subAccountId;
	if @oldaddr_line2 <> @addr_line2
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, 'eaa255c9-e367-4301-bd8c-00eb5fd9edd4', @oldaddr_line2, @addr_line2, @recordtitle, @subAccountId;
	if @oldtown <> @town
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, 'b6cf32d3-11a8-44ba-b8c5-595501272c14', @oldtown, @town, @recordtitle, @subAccountId;
	if @oldcounty <> @county
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, '103d6cd8-89d7-4b20-8281-ba134a10fc2c', @oldcounty, @county, @recordtitle, @subAccountId;
	if @oldpostcode <> @postcode
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, '1c97eb6b-67a9-4a5b-b135-845baf0df3f3', @oldpostcode, @postcode, @recordtitle, @subAccountId;
	if @oldcountryid <> @countryid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, '71a04b4f-898a-43ee-8d5b-bfb694a684fa', @oldcountryid, @countryid, @recordtitle, @subAccountId;
	if @oldswitchboard <> @switchboard
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, 'e219875b-4150-4aac-b0bc-02e63f518ed5', @oldswitchboard, @switchboard, @recordtitle, @subAccountId;
	if @oldfax <> @fax
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, '1e9c5558-7467-4411-a2e9-81ff6310779f', @oldfax, @fax, @recordtitle, @subAccountId;
	if @oldprivate_address <> @private_address
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @addressid, '40418e7f-a9e9-4e7d-b496-995293d0c7e8', @oldprivate_address, @private_address, @recordtitle, @subAccountId;
end

return @id
end
