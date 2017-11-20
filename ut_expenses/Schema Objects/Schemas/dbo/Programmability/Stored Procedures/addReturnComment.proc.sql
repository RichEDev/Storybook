CREATE PROCEDURE [dbo].[addReturnComment] 
 @claimId int,
 @ids IntPK readonly,
 @reason nvarchar(4000),
 @employeeId int
AS
BEGIN

 SET NOCOUNT ON;
 declare @stage int = (select stage from claims_base where claimid = @claimId)

    insert into claimhistory (claimid, datestamp, comment, stage, refnum, employeeid) select @claimId,getDate(),'Expense Returned: ' + @reason,@stage,refnum,@employeeId from savedexpenses where expenseid in (select c1 from @ids)
END