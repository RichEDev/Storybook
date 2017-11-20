CREATE FUNCTION dbo.GetContractProductCount (@contractId int)
RETURNS int
AS
BEGIN
DECLARE @count int

SET @count = (SELECT COUNT([ContractProductId]) FROM contract_productdetails WHERE [ContractId] = @contractId)

RETURN @count
END
