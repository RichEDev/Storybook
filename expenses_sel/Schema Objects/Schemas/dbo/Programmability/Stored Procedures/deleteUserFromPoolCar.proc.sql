


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteUserFromPoolCar]
	@employeeid int,
	@carid int,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @title1 nvarchar(500);
	declare @recordTitle nvarchar(2000);

	select @title1 = username from employees where employeeid = @employeeid;
	set @recordTitle = (select 'Pool Car ' + CAST(@carid AS nvarchar(20)) + ' - User ' + @title1);

    -- Insert statements for procedure here
	DELETE FROM pool_car_users WHERE carid = @carid AND employeeid = @employeeid;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 43, @carid, @recordTitle, null;
END



