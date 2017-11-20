-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE clearDisallowedAddItems
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete FROM additems WHERE subcatid NOT IN (SELECT subcatid FROM rolesubcats INNER JOIN employee_roles ON employee_roles.itemroleid = rolesubcats.roleid WHERE dbo.employee_roles.employeeid = additems.employeeid) 
END
