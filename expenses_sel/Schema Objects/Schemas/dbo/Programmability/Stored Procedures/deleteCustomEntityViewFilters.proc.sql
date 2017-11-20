create PROCEDURE [dbo].[deleteCustomEntityViewFilters]
	-- Add the parameters for the stored procedure here
	@viewID int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM [custom_entity_view_filters] WHERE viewid = @viewID;
END

