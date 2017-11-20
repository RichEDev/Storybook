
CREATE PROCEDURE [dbo].[saveTooltip] (@text NVARCHAR(4000), @tooltipID uniqueidentifier,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @count int
    -- Insert statements for procedure here
	SET @count = (SELECT COUNT(*) FROM customised_help_text WHERE tooltipID = @tooltipID)
	
	DECLARE @helpid int;

	IF @count = 0
		BEGIN
			DECLARE @page NVARCHAR(50);
			DECLARE @description NVARCHAR(200);
			DECLARE @tooltipArea NVARCHAR(3);
			DECLARE @tooltipModuleID INT;
			SELECT @page = page, @description = description, @helpid = helpid, @tooltipArea = tooltipArea, @tooltipModuleID = moduleID FROM [$(metabaseExpenses)].dbo.help_text WHERE tooltipID = @tooltipID;
			INSERT INTO customised_help_text (helpid, page, description, helptext, tooltipID, tooltipArea, moduleID) VALUES (@helpid, @page, @description, @text, @tooltipID, @tooltipArea, @tooltipModuleID);

			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 86, @helpid, @tooltipID, null;
		END
	ELSE
		BEGIN
			declare @oldtext NVARCHAR(4000);
			select @oldtext = helptext, @helpid = helpid from customised_help_text WHERE tooltipID = @tooltipID;
			
			UPDATE customised_help_text SET helptext = @text WHERE tooltipID = @tooltipID;

			if @oldtext <> @text
				begin
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 86, @helpid, @tooltipID, @oldtext, @text, @tooltipID, null;
				end
		END
END

