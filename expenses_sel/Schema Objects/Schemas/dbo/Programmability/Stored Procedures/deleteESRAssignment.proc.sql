


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteESRAssignment]
	@esrAssignID int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @title1 nvarchar(500);
	select @title1 = assignmentNumber from esr_assignments where esrassignid = @esrAssignID;

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'ESR Assignment Number ' + @title1);

    -- Insert statements for procedure here
	delete from esr_assignments where esrassignid = @esrAssignID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @esrAssignID, @recordTitle, null;
END



