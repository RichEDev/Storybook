



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[recordEmployeeModified] 
	@employeeid INT,
	@date DATETIME,
	@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @olddate datetime;
	declare @olduserid int;
	declare @recordtitle nvarchar(2000);
	select @recordtitle = username, @olduserid = modifiedby, @olddate = modifiedon from employees where employeeid = @employeeid;

    -- Insert statements for procedure here
	UPDATE employees SET modifiedon = @date, modifiedby = @userid WHERE employeeid = @employeeid;
	
	if @CUemployeeID > 0
	Begin
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'e27c6957-0435-4177-b1a6-b56459466c40', @olddate, @date, @recordtitle, null;
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '3c749b9d-6e8f-4711-96dc-48c8aaf8abc8', @olduserid, @userid, @recordtitle, null;
	end
END




