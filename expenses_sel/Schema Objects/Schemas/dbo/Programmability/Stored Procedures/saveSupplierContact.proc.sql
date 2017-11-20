CREATE PROCEDURE [dbo].[saveSupplierContact](
@userid int,
@contactid int,
@supplierid int,
@contactname nvarchar(200),
@position nvarchar(200),
@email nvarchar(300),
@mobile nvarchar(30),
@business_addressid int,
@private_addressid int,
@comments ntext,
@maincontact bit,
@CUemployeeID INT,
@CUdelegateID INT)
AS
begin
declare @id int;
declare @count int
declare @subAccountId int;
set @subAccountId = (select subAccountId from supplier_details where supplierId = @supplierid);

if @contactid = 0
begin
	set @count = (SELECT COUNT(*) FROM supplier_contacts where contactname = @contactname and supplierid = @supplierid)
	if @count > 0
		return -1;

	insert into supplier_contacts (supplierid, contactname, position, email, mobile, business_addressid, home_addressid, comments, main_contact, createdon, createdby, modifiedon, modifiedby)
	values (@supplierid, @contactname, @position, @email, @mobile, @business_addressid, @private_addressid, @comments, @maincontact, getdate(), @userid, getdate(), @userid)

	set @id = scope_identity();

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @id, @contactname, @subAccountId;
end
else
begin
	set @count = (SELECT COUNT(*) FROM supplier_contacts where contactname = @contactname and supplierid = @supplierid and contactid <> @contactid)
	if @count > 0
		return -1;

	declare @oldsupplierid int;
	declare @oldcontactname nvarchar(200);
	declare @oldposition nvarchar(200);
	declare @oldemail nvarchar(300);
	declare @oldmobile nvarchar(30);
	declare @oldbusiness_addressid int;
	declare @oldprivate_addressid int;
	--declare @oldcomments ntext;	@oldcomments = comments,
	declare @oldmaincontact bit;
	select @oldsupplierid = supplierid,
		@oldcontactname = contactname,
		@oldposition = position,
		@oldemail = email,
		@oldmobile = mobile,
		@oldbusiness_addressid = business_addressid,
		@oldprivate_addressid = home_addressid,
		@oldmaincontact = main_contact
		from supplier_contacts where contactid = @contactid;

	update supplier_contacts set
	contactname = @contactname,
	supplierid = @supplierid,
	position = @position,
	email = @email,
	mobile = @mobile,
	business_addressid = @business_addressid,
	home_addressid = @private_addressid,
	comments = @comments,
	main_contact = @maincontact,
	modifiedon = getdate(),
	modifiedby = @userid
	where contactid = @contactid

	set @id = @contactid;


	if @oldsupplierid <> @supplierid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @contactid, 'eb5f9ecc-8846-486b-8a17-f9b3be432836', @oldsupplierid, @supplierid, @contactname, @subAccountId;
	if @oldcontactname <> @contactname
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @contactid, '448811ac-3498-47d5-8f82-ba71ae8cf9ed', @oldcontactname, @contactname, @contactname, @subAccountId;
	if @oldposition <> @position
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @contactid, '9d5e44cf-dedc-43e2-a580-ccf868e02fcb', @oldposition, @position, @contactname, @subAccountId;
	if @oldemail <> @email
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @contactid, '915e7773-39f8-4a98-898b-cf00ff33dc2b', @oldemail, @email, @contactname, @subAccountId;
	if @oldmobile <> @mobile
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @contactid, 'b3166234-0c3c-4745-972b-16907ca6d0da', @oldmobile, @mobile, @contactname, @subAccountId;
	if @oldbusiness_addressid <> @business_addressid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @contactid, '94c8f0e7-f3d5-44c8-a352-7d6a0c6ab81d', @oldbusiness_addressid, @business_addressid, @contactname, @subAccountId;
	if @oldprivate_addressid <> @private_addressid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @contactid, '07328dfd-91d9-4f86-809d-40b12ab67c8c', @oldprivate_addressid, @private_addressid, @contactname, @subAccountId;
	--if @oldcomments <> @comments
	--exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @contactid, '448811ac-3498-47d5-8f82-ba71ae8cf9ed', @oldcomments, @comments, @contactname, @subAccountId;
	if @oldmaincontact <> @maincontact
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 54, @contactid, '99706140-588a-49d2-9f44-af274a9096b3', @oldmaincontact, @maincontact, @contactname, @subAccountId;

end

return @id
end
