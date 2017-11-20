
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteCardStatement] (@statementid INT, @employeeID INT, @delegateID INT) 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @count INT;
	SET @count = (SELECT COUNT(*) FROM savedexpenses WHERE transactionid IN (SELECT transactionid FROM [card_transactions] WHERE statementid = @statementid))
	IF @count > 0
		BEGIN
			RETURN -1;
		END

	declare @recordTitle varchar(4000);
	select @recordTitle = [name] from card_statements_base where statementid = @statementid;

    -- Insert statements for procedure here
	DELETE FROM card_statements_base WHERE statementid = @statementid;

	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 17, @statementid, @recordTitle, null;
	RETURN 0;
END
