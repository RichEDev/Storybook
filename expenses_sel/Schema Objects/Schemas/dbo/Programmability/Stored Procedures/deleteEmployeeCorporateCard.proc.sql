

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteEmployeeCorporateCard]
	@corporatecardid INT,
	@userid INT,
	@date datetime,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @employeeid int;
	declare @recordTitle nvarchar(2000);
	DECLARE @cardnumber nvarchar(50);
	SELECT @employeeid = employeeid, @cardnumber = cardnumber FROM dbo.employee_corporate_cards WHERE corporatecardid = @corporatecardid;
	
    delete from employee_corporate_cards where corporatecardid = @corporatecardid;

	set @recordTitle = (select 'Corporate Card Number ' + @cardnumber);
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 17, @corporatecardid, @recordTitle, null;

	UPDATE employees SET modifiedon = @date, modifiedby = @userid WHERE employeeid = @employeeid 
END
