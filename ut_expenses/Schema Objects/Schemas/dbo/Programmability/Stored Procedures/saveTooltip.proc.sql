
CREATE PROCEDURE [dbo].[saveTooltip] 
(
@text NVARCHAR(4000), 
@tooltipID uniqueidentifier,
@CUemployeeID INT,
@CUdelegateID INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @auditInfo NVARCHAR(300);
	DECLARE @page NVARCHAR(50);
	DECLARE @description NVARCHAR(200);

	DECLARE @count int
    -- Insert statements for procedure here
	SET @count = (SELECT COUNT(*) FROM customised_help_text WHERE tooltipID = @tooltipID)
	
	IF @count = 0
		BEGIN
			DECLARE @tooltipArea NVARCHAR(3);
			DECLARE @tooltipModuleID INT;
			SELECT @page = page, @description = [description], @tooltipArea = tooltipArea, @tooltipModuleID = moduleID FROM [$(targetMetabase)].dbo.help_text WHERE tooltipID = @tooltipID;
			
			INSERT INTO customised_help_text (page, description, helptext, tooltipID, tooltipArea, moduleID) VALUES (@page, @description, @text, @tooltipID, @tooltipArea, @tooltipModuleID);

			SET @auditInfo = 'Page: "' + @page + '", Area: "' + @description + '"';

			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 86, 0, @auditInfo, null;
		END
	ELSE
		BEGIN
			declare @oldtext NVARCHAR(4000);
			select @page = page, @description = [description], @oldtext = helptext from customised_help_text WHERE tooltipID = @tooltipID;
			
			UPDATE customised_help_text SET helptext = @text WHERE tooltipID = @tooltipID;

			if @oldtext <> @text
				begin
					SET @auditInfo = 'Page: "' + @page + '", Area: "' + @description + '"';
					
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 86, 0, @tooltipID, @oldtext, @text, @auditInfo, null;
				end
		END
END

