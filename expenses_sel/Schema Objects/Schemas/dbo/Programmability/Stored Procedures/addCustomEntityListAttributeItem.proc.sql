
CREATE PROCEDURE [dbo].[addCustomEntityListAttributeItem]
	@valueid INT,
	@attributeid INT,
	@order INT,
	@item NVARCHAR(50),
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	DECLARE @count INT;
	
	IF @valueid = 0
	BEGIN
		SET @count = (SELECT COUNT(*) FROM custom_entity_attribute_list_items WHERE attributeid = @attributeid AND item = @item);
		
		IF @count = 0
		BEGIN		
			-- SET NOCOUNT ON added to prevent extra result sets from
			-- interfering with SELECT statements.
			SET NOCOUNT ON;

			-- Insert statements for procedure here
			INSERT INTO [custom_entity_attribute_list_items] (
				[attributeid],
				[item],
				[order]
			) VALUES ( @attributeid, @item, @order);
		END
	END
	ELSE
	BEGIN
		SET @count = (SELECT COUNT(*) FROM custom_entity_attribute_list_items WHERE valueid <> valueid AND attributeid = @attributeid AND item = @item);
		
		IF @count = 0
		BEGIN
			UPDATE custom_entity_attribute_list_items SET item = @item, [order] = @order WHERE valueid = @valueid;
		END
	END		
END
