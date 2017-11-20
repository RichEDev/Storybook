


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteProjectcode] 
	@projectcodeid INT, @employeeid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @username NVARCHAR(50)
	SELECT @username = username FROM employees WHERE employeeid = @employeeid;
	
	DECLARE @projectcode NVARCHAR(50)
	SELECT @projectcode = projectcode FROM project_codes WHERE projectcodeid = @projectcodeid;
	
    update [savedexpenses_costcodes] set [projectcodeid] = null where projectcodeid = @projectcodeid
    update employee_costcodes set projectcodeid = null where projectcodeid = @projectcodeid
    delete from project_codes where projectcodeid = @projectcodeid
    
    EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 3, @projectcodeid, @projectcode, null
END



