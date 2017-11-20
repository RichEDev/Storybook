CREATE PROCEDURE [dbo].[saveSupplierCategory] (
@ID int,
@subaccountid int,
@description nvarchar(50),
@employeeId int,
@delegateID int
)
AS
begin
declare @CategoryID int;
declare @count int
declare @recordTitle nvarchar(2000);

if @ID = -1
begin
	set @count = (SELECT COUNT(*) FROM supplier_categories where description = @description and subAccountId = @subaccountid)
	if @count > 0
		return -1;

	insert into supplier_categories (subaccountid, description, createdon, createdby, modifiedon, modifiedby)
	values (@subaccountid, @description, getdate(), @employeeId, getdate(), @employeeId)

	set @CategoryID = scope_identity();
	
	set @recordTitle = 'Supplier Category: ' + @description;
	
	exec addInsertEntryToAuditLog @employeeId, @delegateID, 53, @CategoryID, @recordTitle, @subAccountId;
end
else
begin
	set @count = (SELECT COUNT(*) FROM supplier_categories where description = @description and subaccountid = @subaccountid and categoryid <> @ID)
	if @count > 0
		return -1;

	declare @olddescription nvarchar(50);
	select @olddescription = [description] from supplier_categories where categoryid = @ID;
	
	update supplier_categories set
	description = @description,
	modifiedon = getdate(),
	modifiedby = @employeeId
	where categoryid = @ID

	if @olddescription <> @description
		exec addUpdateEntryToAuditLog @employeeId, @delegateID, 53, @ID, '0F1D0D1A-CDDA-468C-93FF-AB549217D6F6', @olddescription, @description, @recordTitle, @subAccountId;
		
	set @CategoryID = @ID
end

return @CategoryID
end
