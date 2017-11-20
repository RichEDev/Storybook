
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteCorporateCard] (@cardproviderid INT, @employeeID INT, @delegateID INT) 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @count INT;
	SET @count = (SELECT COUNT(*) FROM [card_statements_base] WHERE cardproviderid = @cardproviderid);
	IF @count >0 
		BEGIN
			RETURN -1;
		END
		
    -- Insert statements for procedure here
	DELETE FROM corporate_cards WHERE cardproviderid = @cardproviderid
	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 17, @cardproviderid, @cardproviderid, null;

	RETURN 0;
END
