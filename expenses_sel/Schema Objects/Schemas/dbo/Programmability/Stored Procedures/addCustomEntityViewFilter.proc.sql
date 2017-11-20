create PROCEDURE [dbo].[addCustomEntityViewFilter]
	-- Add the parameters for the stored procedure here
	@viewID INT,
	@fieldID UNIQUEIDENTIFIER,
	@order tinyint,
	@condition tinyint,
	@value nvarchar(150),
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [custom_entity_view_filters] (
		[viewid],
		[fieldid],
		[order],
		[condition],
		[value]
	) VALUES ( @viewID, @fieldID, @order, @condition, @value)
		
END

