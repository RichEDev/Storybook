CREATE PROCEDURE [dbo].[getAssignmentArchiveCount]
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @gracePeriod int
	set @gracePeriod = (select convert(int,stringvalue) from accountproperties where stringkey = 'archiveGracePeriod')
	declare @assignmentCount int
	set @assignmentCount = (select count(*) from esr_assignments where esr_assignments.active = 1 and dateadd(d, @gracePeriod, esr_assignments.FinalAssignmentEndDate) <= getutcdate() and (select count(*) from savedexpenses inner join claims on savedexpenses.claimid = claims.claimid where claims.employeeid = esr_assignments.employeeid and (claims.submitted = 0 or (claims.submitted = 1 and claims.paid = 0))) = 0)
	
	return @assignmentCount
END