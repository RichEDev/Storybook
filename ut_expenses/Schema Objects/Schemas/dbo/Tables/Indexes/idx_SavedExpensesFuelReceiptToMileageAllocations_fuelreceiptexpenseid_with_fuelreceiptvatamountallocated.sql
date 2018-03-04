	CREATE NONCLUSTERED INDEX [idx_SavedExpensesFuelReceiptToMileageAllocations_fuelreceiptexpenseid_with_fuelreceiptvatamountallocated]
	ON [dbo].[SavedExpensesFuelReceiptToMileageAllocations] ([FuelReceiptExpenseId])
	INCLUDE ([FuelReceiptVatAmountAllocated])