CREATE PROCEDURE dbo.GetClaimDefinition
@claimId int
AS
BEGIN
    select top 1 name, [description] from claims_base where claimid = @claimId;
END