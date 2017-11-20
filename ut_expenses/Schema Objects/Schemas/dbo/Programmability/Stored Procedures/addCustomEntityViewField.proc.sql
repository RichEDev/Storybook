CREATE PROCEDURE [dbo].[addCustomEntityViewField]
	-- Add the parameters for the stored procedure here
	@viewid INT,
	@fieldid UNIQUEIDENTIFIER,
	@order TINYINT,
	@joinViaID INT,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [customEntityViewFields] (
		[viewid],
		[fieldid],
		[order],
		[joinViaID]
	) VALUES ( @viewid, @fieldid, @order, @joinViaID)
		
END

