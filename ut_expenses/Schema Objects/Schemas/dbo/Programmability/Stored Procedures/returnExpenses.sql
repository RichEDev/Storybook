CREATE PROCEDURE [dbo].[returnExpenses] 
 @claimId int,
 @ids IntPK readonly,
 @userid int,
 @modifiedon datetime,
 @reason nvarchar (4000)
AS
BEGIN

 SET NOCOUNT ON;

 declare @splitApprovalStage bit
 select @splitApprovalStage = splitApprovalStage from claims_base where claimid = @claimId

 if (select count(expenseid) from savedexpenses where claimid=@claimid) = (select count(c1) from @ids)
  begin
   delete from returnedexpenses where expenseid in (select c1 from @ids)
   update savedexpenses set tempallow = 0, returned = 0, itemCheckerId = null where expenseid in (select c1 from @ids)

   update claims_base set datepaid = null, approved = 0, paid = 0, datesubmitted = null, status = 0, teamid = null, checkerid = null, stage = 0, submitted = 0, splitApprovalStage = 0, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid
   exec dbo.addReturnComment @claimId, @ids, @reason, @userid
  end
    else
  begin
   delete from returnedexpenses where expenseid in (select c1 from @ids)
   insert into returnedexpenses (note, expenseid) select @reason, c1 from @ids
   exec dbo.addReturnComment @claimId, @ids, @reason, @userid
   update savedexpenses set returned = 1 where expenseid in (select c1 from @ids)
  end

 UPDATE [floats] SET settled = 0 WHERE floatid in (select floatid from float_allocations where expenseid in (select c1 from @ids))
END