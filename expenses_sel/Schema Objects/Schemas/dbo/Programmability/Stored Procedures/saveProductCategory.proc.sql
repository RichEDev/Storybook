CREATE procedure [dbo].[saveProductCategory]
@ID int,
@subAccountId int,
@description nvarchar(50),
@employeeId int,
@delegateID int
as
begin
	declare @count int
	declare @returnId int

	if @ID = -1
	begin
		set @count = (select categoryId from productCategories where [description] = @description and subAccountId = @subAccountId)
		if @count >0
			return -1;

		insert into productCategories (subAccountId, [description], [archived], createdon, createdby)
		values (@subAccountId, @description, 0, getdate(), @employeeId)

		set @returnId = scope_identity()
		
		exec addInsertEntryToAuditLog @employeeID, @delegateID, 60, @ID, @description, @subAccountId;
	end
	else
	begin
		set @count = (select categoryId from productCategories where [description] = @description and subAccountId = @subAccountId and categoryId <> @ID)
		if @count > 0
			return -1;

		declare @olddescription nvarchar(50);
		select @olddescription = description from productCategories where categoryId = @ID;
		
		update productCategories set [description] = @description, modifiedon = getdate(), modifiedby = @employeeId
		where categoryId = @ID
 
		set @returnId = @ID
		
		if @olddescription <> @description
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 60, @ID, '7F52AC64-0666-4077-B1F8-1917120306E2', @olddescription, @description, @description, @subAccountId;
	end

	return @returnId
end
