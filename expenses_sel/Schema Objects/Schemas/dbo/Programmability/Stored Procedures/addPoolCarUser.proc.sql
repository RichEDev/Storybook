

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[addPoolCarUser] 
	@employeeid int,
	@carid int,
	@createdon DateTime,
	@createdby int,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @entityID int;
	declare @title1 nvarchar(500);
	declare @title2 nvarchar(500);
	declare @recordTitle nvarchar(2000);

	select @title1 = registration from cars where carid = @carid;
	select @title2 = username from employees where employeeid = @employeeid;

	set @recordTitle = (select 'Pool Car ' + @title1 + ' - User ' + @title2);

    -- Insert statements for procedure here
	INSERT INTO pool_car_users (carid, employeeid, createdon, createdby) VALUES (@carid, @employeeid, @createdon, @createdby);
	set @entityID = scope_identity();

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 43, @entityID, @recordTitle, null;
	
END
