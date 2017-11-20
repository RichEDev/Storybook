CREATE PROCEDURE [dbo].[ESRAssignmentCheck] 
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;
declare @gracePeriod int
set @gracePeriod = (select convert(int,stringvalue) from accountproperties where stringkey = 'archiveGracePeriod')
select distinct esr_assignments.esrassignid, esr_assignments.employeeid, esr_assignments.FinalAssignmentEndDate from savedexpenses inner join claims on savedexpenses.claimid = claims.claimid
inner join esr_assignments on esr_assignments.employeeid = claims.employeeid
where (claims.submitted = 0 or (claims.submitted = 1 and claims.paid = 0))
and esr_assignments.active = 1 and dateadd(d, @gracePeriod, esr_assignments.FinalAssignmentEndDate) <= getutcdate()
END
