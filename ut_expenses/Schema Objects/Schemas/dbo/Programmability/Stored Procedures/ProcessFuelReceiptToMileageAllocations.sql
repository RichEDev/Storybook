CREATE PROCEDURE [dbo].[ProcessFuelReceiptToMileageAllocations]
AS
BEGIN

DECLARE @employeeId int
DECLARE @mileageItemId int
DECLARE @mileageItemDate datetime
DECLARE @mileageItemVatOutstanding money
DECLARE @fuelItemId int
DECLARE @fuelItemVat money
DECLARE @vatAllocated money

DECLARE mileageCursor CURSOR
FOR SELECT expenseid, claims.employeeid, savedexpenses.[date], savedexpenses.vat FROM savedexpenses 
    INNER JOIN subcats ON savedexpenses.subcatid = subcats.subcatid 
    INNER JOIN claims ON savedexpenses.claimid = claims.claimid
	LEFT JOIN SavedExpensesFuelReceiptToMileageAllocations ON SavedExpensesFuelReceiptToMileageAllocations.MileageExpenseId = savedexpenses.expenseid 
    WHERE subcats.mileageapp = 1
	AND claims.paid = 1
	AND savedexpenses.vat > 0
	AND SavedExpensesFuelReceiptToMileageAllocations.FuelReceiptToMileageAllocationId is null
    ORDER BY date ASC

OPEN mileageCursor
FETCH NEXT FROM mileageCursor INTO @mileageItemId, @employeeId, @mileageItemDate, @mileageItemVatOutstanding

WHILE @@fetch_status = 0
BEGIN
	
	
		DECLARE fuelCursor CURSOR 
		FOR SELECT expenseid, max(savedexpenses.vat) - isnull((sum(FuelReceiptVatAmountAllocated)),0) AS vatRemaining  FROM savedexpenses 
        INNER JOIN subcats on savedexpenses.subcatid = subcats.subcatid 
        INNER JOIN claims on savedexpenses.claimid = claims.claimid
		LEFT JOIN SavedExpensesFuelReceiptToMileageAllocations ON SavedExpensesFuelReceiptToMileageAllocations.FuelReceiptExpenseId = savedexpenses.expenseid
        WHERE subcats.calculation = 5 
        AND employeeid = @employeeId
        AND [date] BETWEEN dateadd(week, -6, @mileageItemDate) and @mileageItemDate
		AND savedexpenses.vat > 0
		GROUP BY savedexpenses.date, savedexpenses.expenseid
		HAVING max(savedexpenses.vat) - isnull((sum(FuelReceiptVatAmountAllocated)),0) > 0
        ORDER BY [date] ASC

		OPEN fuelCursor
		FETCH NEXT FROM fuelCursor INTO @fuelItemId, @fuelItemVat
		WHILE @@fetch_status = 0
		BEGIN
			
			
					if @fuelItemVat < @mileageItemVatOutstanding
					BEGIN
						set @vatAllocated = @fuelItemVat
						set @mileageItemVatOutstanding = @mileageItemVatOutstanding - @fuelItemVat
					END
					else
					BEGIN
						set @vatAllocated = @mileageItemVatOutstanding
						set @mileageItemVatOutstanding = 0
						
					END
					
					INSERT INTO SavedExpensesFuelReceiptToMileageAllocations (MileageExpenseId, FuelReceiptExpenseId, FuelReceiptVatAmountAllocated, CreatedOn)
					VALUES (@mileageItemId, @fuelItemId, @vatAllocated, GETDATE())

					if @mileageItemVatOutstanding = 0
						break --everything has been alocated for this expense so don't look through the rest of the vat receipts
			FETCH NEXT FROM fuelCursor INTO @fuelItemId, @fuelItemVat
			
		END
		
		CLOSE fuelCursor
		DEALLOCATE fuelCursor

	FETCH NEXT FROM mileageCursor INTO @mileageItemId, @employeeId, @mileageItemDate, @mileageItemVatOutstanding

END

CLOSE mileageCursor
DEALLOCATE mileageCursor

END
GO


