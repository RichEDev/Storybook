CREATE PROCEDURE [dbo].[deleteCustomEntityListAttributeItems]
	@valueid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM [custom_entity_attribute_list_items] WHERE valueid = @valueid;
END
