


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.updateCarModifiedOn
	@employeeid int,
	@carId int,
	@date datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @carId <> 0
		update cars set modifiedon = @date, modifiedby = @employeeid where carid = @carId;
	
END



