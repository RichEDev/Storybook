CREATE PROCEDURE [dbo].[saveSupplier] (
@userid int,
@supplierid int,
@subaccountid int,
@suppliername nvarchar(150),
@primary_addressid int,
@statusid int,
@categoryid int,
@annual_turnover float,
@supplier_currency int,
@numberofemployees int,
@financial_ye smallint,
@financial_statusid int,
@weburl nvarchar(250),
@suppliercode nvarchar(25),
@supplierEmail nvarchar(300),
@financialStatusLastChecked datetime,
@internalContact nvarchar(250),
@isSupplier bit,
@isReseller bit,
@CUemployeeID INT,
@CUdelegateID INT
)
AS
BEGIN
declare @id int;
declare @count int
if @supplierid = 0
begin
	SET @count = (SELECT COUNT(*) FROM supplier_details WHERE suppliername = @suppliername AND subAccountId = @subaccountid);
	IF @count > 0
		RETURN -1;

	insert into supplier_details (subAccountId,suppliername,primary_addressid,statusid,categoryid,annual_turnover,supplier_currency,numberofemployees,financial_ye,financial_statusid,weburl,suppliercode,createdon,createdby,modifiedon,modifiedby, supplierEmail, financialStatusLastChecked, internalContact, isSupplier, isReseller)
	values (@subaccountid,@suppliername,@primary_addressid,@statusid,@categoryid,@annual_turnover,@supplier_currency,@numberofemployees,@financial_ye,@financial_statusid,@weburl,@suppliercode,getdate(),@userid,getdate(),@userid, @supplierEmail, @financialStatusLastChecked, @internalContact, @isSupplier, @isReseller)
	set @id = scope_identity();

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @id, @suppliername,@subaccountid;
end
else
begin
	SET @count = (SELECT COUNT(*) FROM supplier_details WHERE suppliername = @suppliername and supplierid <> @supplierid AND subAccountId = @subaccountid);
	IF @count > 0
		RETURN -1;

	declare @oldsubaccountid int;
	declare @oldsuppliername nvarchar(150);
	declare @oldprimary_addressid int;
	declare @oldstatusid int;
	declare @oldcategoryid int;
	declare @oldannual_turnover float;
	declare @oldsupplier_currency int;
	declare @oldnumberofemployees int;
	declare @oldfinancial_ye smallint;
	declare @oldfinancial_statusid int;
	declare @oldweburl nvarchar(250);
	declare @oldsuppliercode nvarchar(25);
	declare @oldsupplierEmail nvarchar(300);
	declare @oldfinancialStatusLastChecked datetime;
	declare @oldinternalContact nvarchar(250);
	declare @oldisSupplier bit;
	declare @oldisReseller bit;
	select @oldsubaccountid = subAccountId, 
		@oldsuppliername = suppliername, 
		@oldprimary_addressid = primary_addressid, 
		@oldstatusid = statusid, 
		@oldcategoryid = categoryid, 
		@oldannual_turnover = annual_turnover, 
		@oldsupplier_currency = supplier_currency, 
		@oldnumberofemployees = numberofemployees, 
		@oldfinancial_ye = financial_ye, 
		@oldfinancial_statusid = financial_statusid, 
		@oldweburl = weburl, 
		@oldsuppliercode = suppliercode,
		@oldsupplierEmail = supplierEmail,
		@oldfinancialStatusLastChecked = financialStatusLastChecked,
		@oldinternalContact = internalContact,
		@oldisSupplier = isSupplier,
		@oldisReseller = isReseller
		from supplier_details where supplierid = @supplierid;

	update supplier_details 
	set 
	subAccountId = @subaccountid,
	suppliername = @suppliername,
	primary_addressid = @primary_addressid,
	statusid = @statusid,
	categoryid = @categoryid,
	annual_turnover = @annual_turnover,
	supplier_currency = @supplier_currency,
	numberofemployees = @numberofemployees,
	financial_ye = @financial_ye,
	financial_statusid = @financial_statusid,
	weburl = @weburl,
	suppliercode = @suppliercode,
	modifiedon = getdate(),
	modifiedby = @userid,
	supplierEmail = @supplierEmail,
	financialStatusLastChecked = @financialStatusLastChecked,
	internalContact = @internalContact,
	isSupplier = @isSupplier,
	isReseller = @isReseller
	where supplierid = @supplierid

	set @id = @supplierid

	if @oldsubaccountid <> @subaccountid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '4a7b292d-b501-4dc8-ab81-accf5e0a3dda', @oldsubaccountid, @subaccountid, @suppliername,@subaccountid;
	if @oldsuppliername <> @suppliername
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '3f834ee1-c272-4c42-902b-f37b1fc95dd9', @oldsuppliername, @suppliername, @suppliername,@subaccountid;
	if @oldprimary_addressid <> @primary_addressid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '40418e7f-a9e9-4e7d-b496-995293d0c7e8', @oldprimary_addressid, @primary_addressid, @suppliername,@subaccountid;
	if @oldstatusid <> @statusid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '751928a2-59d8-4a1d-b987-03891610a243', @oldstatusid, @statusid, @suppliername,@subaccountid;
	if @oldcategoryid <> @categoryid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, 'ca9e654c-a5a9-4248-9a13-5e0b9fe6f4ee', @oldcategoryid, @categoryid, @suppliername,@subaccountid;
	if @oldannual_turnover <> @annual_turnover
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '0b0e300b-3ac5-4144-a92a-a02113fa13a3', @oldannual_turnover, @annual_turnover, @suppliername,@subaccountid;
	if @oldsupplier_currency <> @supplier_currency
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '969e9578-b41f-48f0-a7d6-948f7dd625c9', @oldsupplier_currency, @supplier_currency, @suppliername,@subaccountid;
	if @oldnumberofemployees <> @numberofemployees
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '6ef38a97-a4b9-4c58-8702-f3cd636f5ba2', @oldnumberofemployees, @numberofemployees, @suppliername,@subaccountid;
	if @oldfinancial_ye <> @financial_ye
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '417f2d2e-b487-4513-9153-3f95e7100d31', @oldfinancial_ye, @financial_ye, @suppliername,@subaccountid;
	if @oldfinancial_statusid <> @financial_statusid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '1faa3ea3-7905-45c3-9b50-864222fb356b', @oldfinancial_statusid, @financial_statusid, @suppliername,@subaccountid;
	if @oldweburl <> @weburl
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '6c5d6fb1-ca57-473d-a04a-8262557b70c6', @oldweburl, @weburl, @suppliername,@subaccountid;
	if @oldsuppliercode <> @suppliercode
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '3afe84bc-cd2b-42d6-9ece-e21272b69303', @oldsuppliercode, @suppliercode, @suppliername,@subaccountid;
	if @oldsupplierEmail <> @supplierEmail
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, '018273EB-677F-4D9C-87D8-072211FAE8CF', @oldsupplierEmail, @supplierEmail, @suppliername,@subaccountid;
	if @oldfinancialStatusLastChecked <> @financialStatusLastChecked
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, 'C50CB930-67C1-49F4-B05D-E7E38DA6EA40', @oldfinancialStatusLastChecked, @financialStatusLastChecked, @suppliername,@subaccountid;
	if @oldinternalContact <> @internalContact
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, 'BA651E99-ED7B-41B1-A2AE-CF9380B4EF50', @oldinternalContact, @internalContact, @suppliername,@subaccountid;
	if @oldisSupplier <> @isSupplier
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, 'C32839C6-9B80-4C7E-B86E-9AB3E7452216', @oldisSupplier, @isSupplier, @suppliername,@subaccountid;
	if @oldisReseller <> @isReseller
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 52, @supplierid, 'F08EC695-9FDC-4C97-926A-9F8260C45182', @oldisReseller, @isReseller, @suppliername,@subaccountid;
end

return @id
END
