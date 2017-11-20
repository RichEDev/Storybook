


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.updateEmployeeModifiedOn
	@employeeid int,
	@editedEmployeeId int,
	@date datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @editedEmployeeId <> 0
		UPDATE employees SET modifiedon = @date, modifiedby = @employeeid WHERE employeeid = @editedEmployeeId;
	
	
END



