CREATE PROC [dbo].[UpdateApproverLastRemindedDate]
@claimId INT,
@submitted INT
AS
BEGIN
DECLARE @checkers TABLE (Id int not null identity(1,1),checkerid int)

INSERT INTO @checkers
SELECT distinct  ISNULL(dbo.savedexpenses.itemCheckerId, dbo.claims_base.checkerid)
FROM         dbo.claims_base INNER JOIN dbo.savedexpenses ON dbo.savedexpenses.claimid = dbo.claims_base.claimid 
WHERE     (dbo.claims_base.submitted = 1) AND (dbo.claims_base.paid = 0) AND dbo.claims_base.claimid = @claimId AND (dbo.claims_base.status <> 4)


declare @Id int = (select min(Id) from @checkers)
while (@Id is not null) 
begin  

declare @checkerId int = (select checkerid from @checkers where Id = @Id)
exec CheckAndUpdateApproverRemindedDate @claimId, @checkerId, @submitted

set @Id = (select Id from @checkers where Id = @Id + 1)

end
END