CREATE PROCEDURE [dbo].[deleteSupplierCategory] 
(
@ID int,
@employeeId int,
@delegateID int
)
AS
begin
	declare @subAccountId int;
	declare @recordTitle nvarchar(100);
	declare @description nvarchar(50);
	DECLARE @cnt int;
	SET @cnt = 0;
	
	select @description = description, @subAccountId = subAccountId from supplier_categories where categoryid = @ID;
	
	set @recordTitle = 'Supplier Category: ' + @description + ' (ID:' + CAST(@ID as nvarchar) + ' )';
	
	select @cnt = COUNT(supplierid) from supplier_details where categoryid = @ID;

	IF @cnt > 0
		BEGIN
			RETURN -1;
		END
		
	--update supplier_details set categoryid = null where categoryid = @ID

	delete from supplier_categories where categoryid = @ID;
	
	exec addDeleteEntryToAuditLog @employeeId, @delegateID, 53, @ID, @recordTitle, @subAccountId;

	return @cnt
end
