-- Procedure to get the fund limit of expedite customer by its id's.
CREATE PROCEDURE [dbo].GetFundLimit 
@AccountId int
AS
BEGIN
	SELECT FundLimit FROM registeredusers WHERE accountid=@AccountId
END
