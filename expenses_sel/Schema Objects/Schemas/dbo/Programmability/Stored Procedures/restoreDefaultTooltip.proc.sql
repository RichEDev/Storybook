


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[restoreDefaultTooltip] (@tooltipID uniqueidentifier,
@CUemployeeID INT,
@CUdelegateID INT)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @helpid int;
	select @helpid = helpid from customised_help_text WHERE tooltipID = @tooltipID;

    -- Insert statements for procedure here
	DELETE FROM customised_help_text WHERE tooltipID = @tooltipID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 86, @helpid, @tooltipID;
END



