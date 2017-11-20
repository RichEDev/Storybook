CREATE PROCEDURE [dbo].[restoreDefaultTooltip] 
(
@tooltipID uniqueidentifier,
@CUemployeeID INT,
@CUdelegateID INT
)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @page NVARCHAR(50);
	DECLARE @description NVARCHAR(200);
	DECLARE @auditInfo NVARCHAR(300);

	SELECT @page = page, @description = [description] FROM customised_help_text WHERE tooltipID = @tooltipID;

	SET @auditInfo = 'Page: "' + @page + '", Area: "' + @description + '"';

    -- Insert statements for procedure here
	DELETE FROM customised_help_text WHERE tooltipID = @tooltipID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 86, 0, @auditInfo, null;
END


