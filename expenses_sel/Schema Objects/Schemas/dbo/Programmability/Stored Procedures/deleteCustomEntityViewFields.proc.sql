
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteCustomEntityViewFields]
	-- Add the parameters for the stored procedure here
	@viewid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM [custom_entity_view_fields] WHERE viewid = @viewid;
END

