
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[addCustomEntityViewField]
	-- Add the parameters for the stored procedure here
	@viewid INT,
	@fieldid UNIQUEIDENTIFIER,
	@order tinyint,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [custom_entity_view_fields] (
		[viewid],
		[fieldid],
		[order]
	) VALUES ( @viewid, @fieldid, @order)
		
END

