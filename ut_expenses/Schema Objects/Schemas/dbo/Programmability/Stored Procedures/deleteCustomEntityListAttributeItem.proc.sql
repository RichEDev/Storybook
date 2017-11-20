CREATE PROCEDURE [dbo].[deleteCustomEntityListAttributeItem]
@valueid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM customEntityViewFormSelectionMappings WHERE listValue = @valueid;
	
	-- Insert statements for procedure here
	DELETE FROM [customEntityAttributeListItems] WHERE valueid = @valueid;
END
