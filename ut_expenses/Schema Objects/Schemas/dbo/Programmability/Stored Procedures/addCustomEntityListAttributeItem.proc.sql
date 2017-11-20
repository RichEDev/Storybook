
CREATE PROCEDURE [dbo].[addCustomEntityListAttributeItem]
	@valueid INT,
	@attributeid INT,
	@order INT,
	@item NVARCHAR(150),
	@archived BIT,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	DECLARE @count INT;
	
	IF @valueid = 0
	BEGIN
		SET @count = (SELECT COUNT(*) FROM [customEntityAttributeListItems] WHERE attributeid = @attributeid AND item = @item);
		
		IF @count = 0
		BEGIN		
			-- SET NOCOUNT ON added to prevent extra result sets from
			-- interfering with SELECT statements.
			SET NOCOUNT ON;

			-- Insert statements for procedure here
			INSERT INTO [customEntityAttributeListItems] (
				[attributeid],
				[item],
				[order],
				[archived]
			) VALUES ( @attributeid, @item, @order, @archived);
		END
	END
	ELSE
	BEGIN
		SET @count = (SELECT COUNT(*) FROM [customEntityAttributeListItems] WHERE valueid <> @valueid AND attributeid = @attributeid AND item = @item);
		
		IF @count = 0
		BEGIN
			UPDATE [customEntityAttributeListItems] SET item = @item, [order] = @order, [archived] = @archived WHERE valueid = @valueid;
		END
	END		
END
