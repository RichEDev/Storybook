CREATE PROCEDURE [dbo].[GetApproverClaimCount] 
 @employeeId int,
 @isDefaultApprover bit,
 @excludeTeamClaims bit,
 @delegateID int
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

declare @allowEmployeesInOwnSignoff bit
declare @subAccountID int = (select TOP 1 subaccountid from AccountsSubaccounts);
 declare @count int
 
	select @allowEmployeesInOwnSignoff = cast(stringValue as bit) from accountProperties where stringKey = 'AllowEmployeeInOwnSignoffGroup' and subaccountid = @subAccountID

    set @count = (select count(claimid) from checkandpay where (checkerid = @employeeId OR (itemcheckerid is not null and itemcheckerid = @employeeId)) and (@allowEmployeesInOwnSignoff = 1 or (@allowEmployeesInOwnSignoff = 0 and employeeid <> @employeeId AND employeeid <> @delegateID)))

    if @excludeTeamClaims = 0 and (select count(employeeid) from teamemps where employeeid = @employeeId) > 0
		begin
			declare @allowTeamMemberToApproveOwnClaim bit
			select @allowTeamMemberToApproveOwnClaim = cast(stringValue as bit) from accountProperties where stringKey = 'allowTeamMemberToApproveOwnClaim' and subaccountid = @subAccountID
			if @isDefaultApprover = 0
				set @count = @count + (select count(claimid) from unallocatedclaims where (((teamemployeeid = @employeeId and costcodeteamemployeeid is null) or (teamemployeeid is null and costcodeteamemployeeid = @employeeId and itemcheckerid is null))) and (@allowTeamMemberToApproveOwnClaim = 1 or (@allowTeamMemberToApproveOwnClaim = 0 and employeeid <> @employeeId AND employeeid <> @delegateID)))
			else
				set @count = @count + (select count(claimid) from unallocatedclaims where ((((teamemployeeid = @employeeId and costcodeteamemployeeid is null) or (teamemployeeid is null and costcodeteamemployeeid = @employeeId and itemcheckerid is null)) or (itemcheckerid is null and teamemployeeid is null and costcodeteamemployeeid is null))) and (@allowTeamMemberToApproveOwnClaim = 1 or (@allowTeamMemberToApproveOwnClaim = 0 and employeeid <> @employeeId AND employeeid <> @delegateID)))
		end
	return @count
END
