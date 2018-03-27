CREATE TABLE [dbo].[SavedExpensesFuelReceiptToMileageAllocations](
       [FuelReceiptToMileageAllocationId] [int] IDENTITY(1,1) NOT NULL,
       [MileageExpenseId] [int] NOT NULL,
       [FuelReceiptExpenseId] [int] NOT NULL,
       [FuelReceiptVatAmountAllocated] [money] NOT NULL,
       [CreatedOn] [datetime] NOT NULL);
