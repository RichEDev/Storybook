CREATE PROCEDURE [dbo].[deleteProductCategory]
(
@ID int,
@employeeId int,
@delegateID int
)
AS
BEGIN
	declare @title nvarchar(100);
	declare @subAccountId int;
	DECLARE @cnt int;
	SET @cnt = 0;

	select @title = description, @subAccountId = subAccountId from productDetails where productCategoryId = @ID;

	select @cnt = COUNT(productId) from productDetails where productCategoryId = @ID;

	IF @cnt > 0
		BEGIN
			RETURN -1;
		END

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Product Category : ' + @title);
	
	delete from productCategories where categoryId = @ID
	
	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 60, @ID, @recordTitle, @subAccountId;

	return @cnt
end
