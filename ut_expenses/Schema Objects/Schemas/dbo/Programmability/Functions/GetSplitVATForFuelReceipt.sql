CREATE FUNCTION [dbo].[getSplitVATForFuelReceipt](@expenseId INT,@percentage INT) 
RETURNS DECIMAL(18,2)
AS
BEGIN
DECLARE @vat decimal(18,2)
DECLARE @percentageVat DECIMAL(18,2);
SET @vat = (SELECT SUM(FuelReceiptVatAmountAllocated) FROM SavedExpensesFuelReceiptToMileageAllocations WHERE mileageexpenseid = @expenseId)
SET @percentageVat = (@vat * @percentage)/100
RETURN @percentageVat;
END
