Create PROCEDURE [dbo].[SaveClaimHistory]
	@ClaimId int,
	@DateStamp datetime,
	@Comment nvarchar(4000),
	@Stage tinyint,
	@EmployeeId int,
	@CreatedOn datetime

AS
BEGIN
begin
insert into claimhistory (claimid,datestamp,comment,stage,employeeid,createdon)
 values (@ClaimId,@DateStamp,@Comment,@Stage,@EmployeeId,@CreatedOn)
		  SET  @ClaimId = SCOPE_IDENTITY();
end
return @ClaimId
END
 